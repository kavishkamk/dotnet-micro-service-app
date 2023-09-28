using System.ComponentModel.DataAnnotations;

namespace CommandService.Models
{
    public class Command
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string HowTo { get; set; } = null!;
        [Required]
        public int PlatformId { get; set; }
        [Required]
        public string CommandLine { get; set; } = null!;
        public Platform Platform { get; set; } = null!;
    }
}