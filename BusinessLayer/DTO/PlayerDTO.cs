using ServerCode.DAO;

namespace ServerCode.DTO
{
    public record class PlayerDTO
    {
        public string id { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public record class AuctionItemDTO
    {
        public string playerId { get; set; } = null!;
        public string itemName { get; set; } = null!;
        public int pricePerUnit { get; set; }
        public int quantity { get; set; }
        public int TotalPrice => pricePerUnit * quantity;
    }
    public record class PlayerItemDTO
    {
        public string itemName { get; set; } = null!;
        public int quantity { get; set; }
    }
    public record class PlayerDataDTO
    {
        public string? playerId { get; set; }
        public int gold { get; set; }
        public string? dictionary { get; set; }
        public int weaponLevel { get; set; }
        public int wrmorLevel { get; set; }
    }
    public record class BuyerDTO
    {
        public string? buyerId { get; set; }
        public int buyCount { get; set; }
        public AuctionItemDAO itemInfo { get; set; } = null!;
        public int NeededMoney => buyCount * itemInfo.pricePerUnit;
    }
    public record class DictionaryUpgradeDTO
    {
        public string? dictionaryKey { get; set; }
        public int level { get; set; }
    }
    public enum EquipType
    {
        Weapon = 0,
        Armor = 1
    }
}
