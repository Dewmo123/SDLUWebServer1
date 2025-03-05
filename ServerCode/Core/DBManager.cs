using Microsoft.EntityFrameworkCore;
using ServerCode.Data;
using ServerCode.Models;

namespace ServerCode.Core
{
    public class DBManager : Singleton<DBManager>
    {
        private GameDbContext _context;

        public void ConnectDB(string connectionString)
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Build();
            _context = new GameDbContext(options);
        }

        #region PlayerControl
        public bool CheckIDDuplication(string playerId)
        {
            return _context.PlayerData.Any(p => p.PlayerId == playerId);
        }

        public bool SignUp(string playerId, string password)
        {
            if (CheckIDDuplication(playerId))
                return false;

            var player = new PlayerData
            {
                PlayerId = playerId,
                Password = password
            };

            _context.PlayerData.Add(player);
            return _context.SaveChanges() > 0;
        }

        public bool LogIn(string playerId, string password)
        {
            var player = _context.PlayerData
                .FirstOrDefault(p => p.PlayerId == playerId);
            return player?.Password == password;
        }
        #endregion

        #region ItemControl
        public bool AddItemInfo(string itemName, ItemType type, int maxStack)
        {
            var item = new ItemData
            {
                ItemName = itemName,
                ItemType = type,
                MaxStack = maxStack
            };

            _context.ItemData.Add(item);
            return _context.SaveChanges() > 0;
        }

        public bool RemoveItemInfo(int itemId)
        {
            var item = _context.ItemData.Find(itemId);
            if (item == null) return false;

            _context.ItemData.Remove(item);
            return _context.SaveChanges() > 0;
        }

        public List<ItemInfo> GetItemInfos()
        {
            return _context.ItemData
                .Select(i => new ItemInfo(i.ItemId, i.ItemName, i.ItemType, i.MaxStack))
                .ToList();
        }

        public bool AddItemToPlayer(string playerId, int itemId, int amount)
        {
            var playerItem = _context.PlayerItemData
                .FirstOrDefault(pi => pi.PlayerId == playerId && pi.ItemId == itemId);

            if (playerItem != null)
            {
                int newQuantity = playerItem.Quantity + amount;
                if (newQuantity < 0)
                    return false;

                if (newQuantity == 0)
                {
                    _context.PlayerItemData.Remove(playerItem);
                }
                else
                {
                    playerItem.Quantity = newQuantity;
                }
            }
            else if (amount > 0)
            {
                var newPlayerItem = new PlayerItemData
                {
                    PlayerId = playerId,
                    ItemId = itemId,
                    Quantity = amount
                };
                _context.PlayerItemData.Add(newPlayerItem);
            }

            return _context.SaveChanges() > 0;
        }
        #endregion
    }
}
