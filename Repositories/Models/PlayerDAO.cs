namespace ServerCode.DAO
{
    public record class PlayerDAO
    {
        public string id { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public record class AuctionItemDAO
    {
        public string playerId { get; set; } = null!;
        public string? itemName { get; set; }
        public int pricePerUnit { get; set; }
        public int quantity { get; set; }
        public int TotalPrice => pricePerUnit * quantity;
    }
    public record class PlayerItemDAO
    {
        public string playerId { get; set; } = null!;
        public int quantity { get; set; }
        public string? itemName { get; set; }
        public PlayerItemDAO()
        {

        }
        //생성자
        public PlayerItemDAO(AuctionItemDAO auction)
        {
            playerId = auction.playerId;
            quantity = auction.quantity;
            itemName = auction.itemName;
        }
        public PlayerItemDAO(string pId, int q)
        {
            playerId = pId;
            quantity = q;
        }
    }
    public record class PlayerDataDAO
    {
        public string? playerId { get; set; }
        public int gold { get; set; }
        public string? dictionary { get; set; }
    }
}
