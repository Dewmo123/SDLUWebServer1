namespace ServerCode.DTO
{
    public record class PlayerInfoDTO
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
    public record class PlayerItemInfoDTO
    {
        public string playerId { get; set; } = null!;
        public string itemName { get; set; } = null!;
        public int quantity { get; set; }
    }
    public record class PlayerDataInfoDTO
    {
        public string? playerId { get; set; }
        public int gold { get; set; }
        public string? dictionary { get; set; }
    }
    public record class BuyerInfoDTO
    {
        public string? buyerId { get; set; }
        public int buyCount { get; set; }
        public AuctionItemDTO itemInfo { get; set; } = null!;
        public int NeededMoney => buyCount * itemInfo.pricePerUnit;
    }
    public record class DictionaryUpgradeDTO
    {
        public string? dictionaryKey { get; set; }
        public int level { get; set; }
    }
}
