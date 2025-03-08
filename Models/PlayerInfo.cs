namespace ServerCode.Models
{
    public class PlayerInfo
    {
        public string id { get; set; } = null!;
        public string password { get; set; } = null!;
    }
    public class AuctionItemInfo
    {
        public string playerId { get; set; } = null!;
        public int itemId { get; set; }
        public int pricePerUnit { get; set; }
        public int quantity { get; set; }
    }
    public class PlayerItemInfo
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
        public PlayerItemInfo(string pId,int iId,int q)
        {
            playerId = pId;
            itemId = iId;
            quantity = q;
        }
    }
}
