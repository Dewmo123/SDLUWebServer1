using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerCode.Models
{
    public class PlayerItemData
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(8)]
        public string PlayerId { get; set; } = null!;
        [Key]
        [Column(Order = 1)]
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("PlayerId")]
        public PlayerData Player { get; set; } = null!;
        [ForeignKey("ItemId")]
        public ItemData Item { get; set; } = null!;
    }
} 