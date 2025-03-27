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
            using var transaction = await conn.BeginTransactionAsync();
            try
            {
                var playerItemInfo = new PlayerItemInfo() { playerId = auctionItemInfo.playerId, itemName = auctionItemInfo.itemName, quantity = 0 };

                playerItemInfo = await _repositoryManager.PlayerItems.GetItemByPrimaryKeysAsync(playerItemInfo, conn);
                var remainItemInfo = await _repositoryManager.AuctionItems.GetItemByPrimaryKeysAsync(auctionItemInfo, conn);

                if (playerItemInfo == null)
                    return false;

                int remainQuantity = playerItemInfo.quantity - auctionItemInfo.quantity;
                if (remainQuantity < 0)
                    return false;
                bool success = await _repositoryManager.PlayerItems.UpdateAsync(
                    new PlayerItemInfo() { playerId = auctionItemInfo.playerId, itemName = auctionItemInfo.itemName, quantity = remainQuantity }
                    , conn, transaction);
                Console.WriteLine("Update" + success);

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
            await using MySqlConnection conn = new MySqlConnection(_dbAddress);
            await conn.OpenAsync();
            await using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                PlayerDataInfo playerInfo = new() { playerId = buyerInfo.buyerId, gold = 0 };
                playerInfo = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(playerInfo, conn);

                AuctionItemInfo? auctionItem = buyerInfo.itemInfo;
                auctionItem = await _repositoryManager.AuctionItems.GetItemByPrimaryKeysAsync(auctionItem, conn);
                Console.WriteLine($"auctionItemName: " + auctionItem.itemName);
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
                sellerInfo = await _repositoryManager.PlayerData.GetItemByPrimaryKeysAsync(sellerInfo, conn);
                sellerInfo.gold += buyerInfo.NeededMoney;

                success &= await _repositoryManager.PlayerData.UpdateAsync(sellerInfo, conn, transaction);
                success &= await _repositoryManager.PlayerData.UpdateAsync(playerInfo, conn, transaction);
                if (remain == 0)
                    success &= await _repositoryManager.AuctionItems.DeleteWithPrimaryKeysAsync(auctionItem, conn, transaction);
                else
                    success &= await _repositoryManager.AuctionItems.UpdateAsync(auctionItem, conn, transaction);

                PlayerItemInfo itemInfo = new() { itemName = auctionItem.itemName, playerId = playerInfo.playerId, quantity = buyerInfo.buyCount };
                success &= await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(itemInfo, conn, transaction);
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
            using MySqlTransaction transaction = await conn.BeginTransactionAsync();
            try
            {
                bool success = true;
                success &= await _repositoryManager.AuctionItems.DeleteWithPrimaryKeysAsync(auctionItemInfo, conn, transaction);
                PlayerItemInfo playerItemInfo = new(auctionItemInfo);
                success &= await _repositoryManager.PlayerItems.CheckConditionAndChangePlayerItem(playerItemInfo, conn, transaction);

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
