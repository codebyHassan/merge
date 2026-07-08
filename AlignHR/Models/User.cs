using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(150)]
        public string? LastName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? UsrIsActive { get; set; }


        public int createdby { get; set; }
        public DateTime createat { get; set; } = DateTime.Now;
        public int updatedby { get; set; }
        public DateTime updateat { get; set; }


        // Sarmad stuff 

              // ===== Employee Linkage =====

        /// <summary>
        /// Links this User to an Employee record for workflow inbox resolution.
        /// </summary>
        /// 
        /// 
        /// 
        
        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public int? DmsDefinationFK { get; set; }
        [ForeignKey("DmsDefinationFK")]
        public DmsDefinationFile? DmsDefinationFile { get; set; }

    }
}
