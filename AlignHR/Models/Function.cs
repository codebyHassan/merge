using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class Function
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string? Name { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [Required]
        [StringLength(500)]
        public string? route { get; set; }

        public int createdby { get; set; }

        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }

        public DateTime updateat { get; set; }








    }
}
