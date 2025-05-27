namespace ServerCode.DAO
{
    public record class PlayerVO
    {
        public string id { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public record class AuctionItemVO
    {
        public string playerId { get; set; } = null!;
        public string? itemName { get; set; }
        public int pricePerUnit { get; set; }
        public int quantity { get; set; }
        public int TotalPrice => pricePerUnit * quantity;
    }
    public record class PlayerItemVO
    {
        public string playerId { get; set; } = null!;
        public int quantity { get; set; }
        public string? itemName { get; set; }
        public PlayerItemVO()
        {

        }
        //생성자
        public PlayerItemVO(AuctionItemVO auction)
        {
            playerId = auction.playerId;
            quantity = auction.quantity;
            itemName = auction.itemName;
        }
        public PlayerItemVO(string pId, int q)
        {
            playerId = pId;
            quantity = q;
        }
    }
    public record class PlayerDataVO
    {
        public string? playerId { get; set; }
        public int gold { get; set; }
        public string? dictionary { get; set; }
        public int weaponLevel { get; set; }
        public int armorLevel { get; set; }
    }
}
