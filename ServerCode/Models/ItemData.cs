using System.ComponentModel.DataAnnotations;

namespace ServerCode.Models
{
    public class ItemData
    {
        [Key]
        public int ItemId { get; set; }
        [MaxLength(50)]
        public string ItemName { get; set; } = null!;
        public ItemType ItemType { get; set; }
        public int MaxStack { get; set; }
    }
} 