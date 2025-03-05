using System.ComponentModel.DataAnnotations;

namespace ServerCode.Models
{
    public class PlayerData
    {
        [Key]
        [MaxLength(8)]
        public string PlayerId { get; set; } = null!;
        [MaxLength(20)]
        public string Password { get; set; } = null!;
    }
} 