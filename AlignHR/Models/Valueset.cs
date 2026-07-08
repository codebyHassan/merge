using System.ComponentModel.DataAnnotations;

namespace AlignHR.Models
{
    public class Valueset
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string GroupName { get; set; }   // EmploymentType / EmploymentStatus

        [Required]
        public string Name { get; set; }        // FullTime, Active etc

        public bool IsActive { get; set; } = true;
    }
}
