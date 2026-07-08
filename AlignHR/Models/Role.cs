using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]  
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }
    }
}
