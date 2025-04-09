namespace ServerCode.DAO
{
    //Table과 동일
    public class ItemInfo
    {
        public string itemName;
        public ItemType itemType;
        public ItemInfo()
        {

        }
        public ItemInfo(string name, ItemType type )
        {
            itemName = name;
            itemType = type;
        }
    }
    public class ItemInfos
    {
        public List<ItemInfo> items;
        public ItemInfos(List<ItemInfo> itemInfos)
        {
            items = itemInfos;
        }
    }
}
