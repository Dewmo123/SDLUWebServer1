using MySqlConnector;
using ServerCode.Models;
using static Repositories.DBConfig;

namespace Repositories
{

    public class DBManager
    {
        string _dbAddress = null!;
        private UnitOfWork _unitOfWork = null!;


        public DBManager(string connectionAddress)
        {
            _dbAddress = connectionAddress;
            _unitOfWork = new UnitOfWork(connectionAddress);
        }

        #region PlayerControl


        public async Task<bool> SignUp(PlayerInfo playerInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                await conn.OpenAsync();
                using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        var info = await _unitOfWork.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, conn, transaction);
                        if (info.id == playerInfo.id)
                        {
                            Console.WriteLine($"{info.id}:Duplicate");
                            return false;
                        }
                        bool success = await _unitOfWork.PlayerInfos.AddAsync(playerInfo, conn, transaction);
                        success &= await _unitOfWork.PlayerData.AddAsync(new PlayerDataInfo() { playerId = playerInfo.id, gold = 0 }, conn, transaction);
                        await transaction.CommitAsync();
                        Console.WriteLine(success);
                        return success;
                    }
                    catch (MySqlException)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    catch (InvalidOperationException)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    finally
                    {
                        await conn.CloseAsync();
                    }
                }
            }
        }
        public async Task<bool> LogIn(PlayerInfo playerInfo)
        {

            using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                var info = await _unitOfWork.PlayerInfos.GetItemByPrimaryKeysAsync(playerInfo, conn, transaction);
                await transaction.CommitAsync();
                return info.password == playerInfo.password;
            }
            catch (MySqlException)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        #endregion

        #region ItemControl
        public async Task<bool> AddItemInfo(ItemInfo itemInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                await conn.OpenAsync();
                using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        bool success = await _unitOfWork.ItemInfos.AddAsync(itemInfo, conn, transaction);
                        await transaction.CommitAsync();
                        return success;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
            }
        }
        public bool RemoveItemInfo(int itemId)
        {
            return true;
        }
        public List<ItemInfo> GetItemInfos()
        {
            return null!;
        }
        #endregion

        #region PlayerItemControl
        public async Task<bool> ChangePlayerItemQuantityAsync(PlayerItemInfo itemInfo)
        {
            using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                return await _unitOfWork.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, conn, transaction);
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.CommitAsync();
            }
        }



        private bool DeleteItem(MySqlConnection conn, MySqlTransaction transaction,
            PlayerItemInfo itemInfo)
        {
            return true;
        }
        #endregion

        #region AuctionControl
        public async Task<bool> AddItemToAuction(AuctionItemInfo auctionItemInfo)
        {
            using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var playerItemInfo = new PlayerItemInfo() { playerId = auctionItemInfo.playerId, itemId = auctionItemInfo.itemId, quantity = 0 };

                playerItemInfo = await _unitOfWork.PlayerItems.GetItemByPrimaryKeysAsync(playerItemInfo, conn, transaction);
                var remainItemInfo = await _unitOfWork.AuctionItems.GetItemByPrimaryKeysAsync(auctionItemInfo, conn, transaction);

                if (playerItemInfo == null)
                    return false;

                int remainQuantity = playerItemInfo.quantity - auctionItemInfo.quantity;
                if (remainQuantity < 0)
                    return false;
                bool success = await _unitOfWork.PlayerItems.UpdateAsync(new PlayerItemInfo() { playerId = auctionItemInfo.playerId, itemId = auctionItemInfo.itemId, quantity = remainQuantity }, conn, transaction);
                Console.WriteLine("Update" + success);

                if (remainItemInfo == null && auctionItemInfo.quantity > 0)
                {
                    success &= await _unitOfWork.AuctionItems.AddAsync(auctionItemInfo, conn, transaction);
                    if (success) await transaction.CommitAsync();
                    else await transaction.RollbackAsync();
                    return success;
                }

                int quantity = remainItemInfo.quantity + auctionItemInfo.quantity;
                if (quantity < 0)
                    return false;
                AuctionItemInfo newItemInfo = new()
                {
                    itemId = remainItemInfo.itemId,
                    playerId = remainItemInfo.playerId,
                    quantity = quantity,
                    pricePerUnit = remainItemInfo.pricePerUnit
                };
                success &= await _unitOfWork.AuctionItems.UpdateAsync(newItemInfo, conn, transaction);
                Console.WriteLine("Update" + success);
                if (success) await transaction.CommitAsync();
                else await transaction.RollbackAsync();
                return success;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> PurchaseItemInAuction(BuyerInfo buyerInfo)
        {
            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                await conn.OpenAsync();
                using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        PlayerDataInfo playerInfo = new() { playerId = buyerInfo.buyerId, gold = 0 };
                        playerInfo = await _unitOfWork.PlayerData.GetItemByPrimaryKeysAsync(playerInfo, conn, transaction);

                        AuctionItemInfo? auctionItem = buyerInfo.itemInfo;
                        auctionItem = await _unitOfWork.AuctionItems.GetItemByPrimaryKeysAsync(auctionItem, conn, transaction);
                        Console.WriteLine($"auctionItemId: " + auctionItem.itemId);
                        if (playerInfo == null || auctionItem == null)
                            return false;

                        if (playerInfo.gold < buyerInfo.NeededMoney || auctionItem.quantity < buyerInfo.buyCount)
                            return false;

                        //돈 빼고 아이템 추가하고 옥션에서 아이템 빼고 리턴
                        bool success = true;

                        playerInfo.gold -= buyerInfo.NeededMoney;
                        int remain = auctionItem.quantity -= buyerInfo.buyCount;
                        Console.WriteLine(auctionItem.quantity);

                        PlayerDataInfo sellerInfo = new() { playerId = auctionItem.playerId, gold = 0 };
                        sellerInfo = await _unitOfWork.PlayerData.GetItemByPrimaryKeysAsync(sellerInfo, conn, transaction);
                        sellerInfo.gold += buyerInfo.NeededMoney;

                        success &= await _unitOfWork.PlayerData.UpdateAsync(sellerInfo, conn, transaction);
                        success &= await _unitOfWork.PlayerData.UpdateAsync(playerInfo, conn, transaction);
                        if (remain == 0)
                            success &= await _unitOfWork.AuctionItems.DeleteWithPrimaryKeysAsync(auctionItem, conn, transaction);
                        else
                            success &= await _unitOfWork.AuctionItems.UpdateAsync(auctionItem, conn, transaction);

                        PlayerItemInfo itemInfo = new() { itemId = auctionItem.itemId, playerId = playerInfo.playerId, quantity = buyerInfo.buyCount };
                        success &= await _unitOfWork.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, conn, transaction);
                        if (success)
                            await transaction.CommitAsync();
                        else
                            await transaction.RollbackAsync();
                        return success;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return false;
                    }
                }
            }
        }
        public async Task<bool> CancelAuctionItem(AuctionItemInfo auctionItemInfo)
        {
            using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                auctionItemInfo = await _unitOfWork.AuctionItems.GetItemByPrimaryKeysAsync(auctionItemInfo, conn, transaction);
                if (auctionItemInfo == null)
                    return false;
                bool success = true;
                success &= await _unitOfWork.AuctionItems.DeleteWithPrimaryKeysAsync(auctionItemInfo, conn, transaction);
                PlayerItemInfo playerItemInfo = new(auctionItemInfo);
                success &= await _unitOfWork.PlayerItems.CheckConditionAndChangePlayerItem(playerItemInfo, conn, transaction);

                if (success)
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();
                return success;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return false;
            }
        }
        private bool AddNewItemToAuction(MySqlConnection conn, MySqlTransaction transaction, AuctionItemInfo auctionItemInfo)
        {
            return true;
        }

        private bool AddAuctionItemQuantity(MySqlConnection conn, MySqlTransaction transaction, AuctionItemInfo auctionItemInfo)
        {

            return true;
        }
        #endregion

    }

}
