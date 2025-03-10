namespace ServerCode.Models
{
    public record class PlayerInfo
    {
        public string id { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public record class AuctionItemInfo
    {
        public string playerId { get; set; } = null!;
        public int itemId { get; set; }
        public int pricePerUnit { get; set; }
        public int quantity { get; set; }
        public int TotalPrice => pricePerUnit * quantity;
    }
    public record class PlayerItemInfo
    {
        public string playerId { get; set; } = null!;
        public int itemId { get; set; }
        public int quantity { get; set; }
        public PlayerItemInfo()
        {

        }
        //생성자
        public PlayerItemInfo(AuctionItemInfo auction)
        {
            playerId = auction.playerId;
            itemId = auction.itemId;
            quantity = auction.quantity;
        }
        public PlayerItemInfo(string pId, int iId, int q)
        {
            playerId = pId;
            itemId = iId;
            quantity = q;
        }
    }
    public record class PlayerGoldInfo
    {
        public string? playerId { get; set; }
        public int gold { get; set; }
    }
    public record class BuyerInfo
    {
        public string? buyerId { get; set; }
        public int buyCount { get; set; }
        public AuctionItemInfo? itemInfo { get; set; }
        public int NeededMoney => buyCount * itemInfo.pricePerUnit;
    }
}
