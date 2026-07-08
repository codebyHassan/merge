using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Code { get; set; }

     
        public string? Description { get; set; }
        [Required]
        public string? Owner { get; set; }

        public string? Role { get; set; }
        public int createdby { get; set; }

        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }

        public DateTime updateat { get; set; }

        // Navigation property for Employees
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();






    }
}
