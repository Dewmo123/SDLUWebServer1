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
                        var info = await _unitOfWork.PlayerInfos.GetByIdAsync(playerInfo, conn, transaction);
                        if (info.id == playerInfo.id)
                        {
                            Console.WriteLine($"{info.id}:Duplicate");
                            return false;
                        }
                        bool success = await _unitOfWork.PlayerInfos.AddAsync(playerInfo, conn, transaction);
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

            using (MySqlConnection conn = new MySqlConnection(_dbAddress))
            {
                await conn.OpenAsync();
                using (MySqlTransaction transaction = await conn.BeginTransactionAsync())
                {
                    try
                    {
                        var info = await _unitOfWork.PlayerInfos.GetByIdAsync(playerInfo, conn, transaction);
                        await transaction.CommitAsync();
                        return info.password == playerInfo.password;
                    }
                    catch (MySqlException)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
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
                var info = await _unitOfWork.PlayerItems.GetByIdAsync(itemInfo, conn, transaction);
                if (info == null && itemInfo.quantity > 0)
                    return await _unitOfWork.PlayerItems.AddAsync(itemInfo, conn, transaction);

                int quantity = info.quantity + itemInfo.quantity;
                if (quantity < 0)
                    return false;

                PlayerItemInfo newItemInfo = new()
                {
                    itemId = info.itemId,
                    playerId = info.playerId,
                    quantity = quantity
                };
                return await _unitOfWork.PlayerItems.UpdateAsync(newItemInfo, conn, transaction);
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
                var info = await _unitOfWork.AuctionItems.GetByIdAsync(auctionItemInfo, conn, transaction);
                if (info == null && auctionItemInfo.quantity > 0)
                    return await _unitOfWork.AuctionItems.AddAsync(auctionItemInfo, conn, transaction);

                int quantity = info.quantity + auctionItemInfo.quantity;
                if (quantity < 0)
                    return false;

                AuctionItemInfo newItemInfo = new()
                {
                    itemId = info.itemId,
                    playerId = info.playerId,
                    quantity = quantity,
                    pricePerUnit = auctionItemInfo.pricePerUnit
                };
                return await _unitOfWork.AuctionItems.UpdateAsync(newItemInfo, conn, transaction);
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


        private bool AddNewItemToAuction(MySqlConnection conn, MySqlTransaction transaction, AuctionItemInfo auctionItemInfo)
        {
            return true;
        }

        private bool AddAuctionItemQuantity(MySqlConnection conn, MySqlTransaction transaction, AuctionItemInfo auctionItemInfo)
        {

            return true;
        }

        private MySqlDataReader CheckAlreadyExistInAuction(MySqlConnection conn, MySqlTransaction transaction, string playerId, int itemId)
        {
            MySqlCommand checkAlreadyExist = new MySqlCommand(Queries.GetAuctionItemById, conn, transaction);
            checkAlreadyExist.Parameters.AddWithValue("@playerId", playerId);
            checkAlreadyExist.Parameters.AddWithValue("@itemId", itemId);
            var table = checkAlreadyExist.ExecuteReader();
            return table;
        }
        #endregion

    }

}
