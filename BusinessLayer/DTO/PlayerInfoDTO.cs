namespace ServerCode.DTO
{
    public record class PlayerInfoDTO
    {
        public string id { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public record class AddToAuctionItemDTO
    {
        public string playerId { get; set; } = null!;
        public int pricePerUnit { get; set; }
        public int quantity { get; set; }
        public int TotalPrice => pricePerUnit * quantity;
    }
    public record class PlayerItemInfo
    {
        public string playerId { get; set; } = null!;
        public int quantity { get; set; }
    }
    public record class PlayerDataInfo
    {
        public string? playerId { get; set; }
        public int gold { get; set; }
    }
    public record class BuyerInfo
    {
        public string? buyerId { get; set; }
        public int buyCount { get; set; }
        public AddToAuctionItemDTO itemInfo { get; set; } = null!;
        public int NeededMoney => buyCount * itemInfo.pricePerUnit;
    }
}
