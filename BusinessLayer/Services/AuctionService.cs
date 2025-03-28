using MySqlConnector;
using Repositories;
using ServerCode.Models;

namespace BusinessLayer.Services
{
    public class AuctionService : Service
    {
        public AuctionService(RepositoryManager repo, string dbAddress) : base(repo, dbAddress)
        {
        }

        public async Task<bool> AddItemToAuction(AuctionItemInfo auctionItemInfo)
        {
            await using var conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            var playerItemInfo = new PlayerItemInfo() { playerId = auctionItemInfo.playerId, itemName = auctionItemInfo.itemName, quantity = 0 };

            playerItemInfo = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(playerItemInfo, conn);
            var remainItemInfo = await _repositoryManager.AuctionItems.GetItemByPrimaryKeysAsync(auctionItemInfo, conn);

            if (playerItemInfo == null)
                return false;

            int remainQuantity = playerItemInfo.quantity - auctionItemInfo.quantity;
            if (remainQuantity < 0)
            {
                await conn.CloseAsync();
                return false;
            }
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                bool success = await _repositoryManager.PlayerItems.UpdateAsync(
                    new PlayerItemInfo() { playerId = auctionItemInfo.playerId, itemName = auctionItemInfo.itemName, quantity = remainQuantity }
                    , conn, transaction);

                if (remainItemInfo == null && auctionItemInfo.quantity > 0)
                {
                    success &= await _repositoryManager.AuctionItems.AddAsync(auctionItemInfo, conn, transaction);
                    if (success) await transaction.CommitAsync();
                    else await transaction.RollbackAsync();
                    return success;
                }

                int quantity = remainItemInfo.quantity + auctionItemInfo.quantity;
                if (quantity < 0)
                    return false;
                AuctionItemInfo newItemInfo = new()
                {
                    itemName = remainItemInfo.itemName,
                    playerId = remainItemInfo.playerId,
                    quantity = quantity,
                    pricePerUnit = remainItemInfo.pricePerUnit
                };
                success &= await _repositoryManager.AuctionItems.UpdateAsync(newItemInfo, conn, transaction);
                Console.WriteLine("Auction Update: " + success);
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
            try
            {
                await using MySqlConnection conn = new MySqlConnection(_dbAddress);
                await conn.OpenAsync();

                PlayerDataInfo buyer = new() { playerId = buyerInfo.buyerId, gold = 0 };
                buyer = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(buyer, conn);

                AuctionItemInfo? auctionItem = buyerInfo.itemInfo;
                auctionItem = await _repositoryManager.AuctionItems.GetItemByPrimaryKeysAsync(auctionItem, conn);

                if (buyer == null || auctionItem == null)
                {
                    await conn.CloseAsync();
                    return false;
                }

                if (buyer.gold < buyerInfo.NeededMoney || auctionItem.quantity < buyerInfo.buyCount)
                {
                    await conn.CloseAsync();
                    return false;
                }

                //돈 빼고 아이템 추가하고 옥션에서 아이템 빼고 리턴
                bool success = true;


                PlayerDataInfo sellerInfo = new() { playerId = auctionItem.playerId, gold = 0 };
                sellerInfo = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(sellerInfo, conn);

                PlayerItemInfo itemInfo = new() { itemName = auctionItem.itemName, playerId = buyer.playerId, quantity = buyerInfo.buyCount };
                var remainItem = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(itemInfo, conn);

                await using MySqlTransaction transaction = await conn.BeginTransactionAsync();

                success &= await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, remainItem, conn, transaction);

                sellerInfo.gold += buyerInfo.NeededMoney;
                success &= await _repositoryManager.PlayerData.UpdateAsync(sellerInfo, conn, transaction);

                buyer.gold -= buyerInfo.NeededMoney;
                success &= await _repositoryManager.PlayerData.UpdateAsync(buyer, conn, transaction);

                int remain = auctionItem.quantity -= buyerInfo.buyCount;
                if (remain == 0)
                    success &= await _repositoryManager.AuctionItems.DeleteWithPrimaryKeysAsync(auctionItem, conn, transaction);
                else
                    success &= await _repositoryManager.AuctionItems.UpdateAsync(auctionItem, conn, transaction);

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
        public async Task<bool> CancelAuctionItem(AuctionItemInfo auctionItemInfo)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            auctionItemInfo = await _repositoryManager.AuctionItems.GetItemByPrimaryKeysAsync(auctionItemInfo, conn);
            if (auctionItemInfo == null)
            {
                await conn.CloseAsync();
                return false;
            }
            PlayerItemInfo playerItemInfo = new(auctionItemInfo);
            PlayerItemInfo remainItem = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(playerItemInfo, conn);
            using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                bool success = true;
                success &= await _repositoryManager.AuctionItems.DeleteWithPrimaryKeysAsync(auctionItemInfo, conn, transaction);
                success &= await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(playerItemInfo, remainItem, conn, transaction);

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
        public async Task<List<AuctionItemInfo>> GetAuctionItemByItemName(string itemName)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            List<AuctionItemInfo> datas = await _repositoryManager.AuctionItems.GetItemsByItemName(itemName, conn);
            return datas;
        }
        public async Task<List<AuctionItemInfo>> GetAuctionnItemByPlayerId(string playerId)
        {
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            var datas = await _repositoryManager.AuctionItems.GetItemsByPlayerId(playerId, conn);
            return datas;
        }
    }
}
