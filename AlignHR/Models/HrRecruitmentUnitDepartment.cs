using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    public class HrRecruitmentUnitDepartment
    {
        [Key]
        [Column(TypeName = "numeric(18,0)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal RecruitmentUnitId { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal DepartmentId { get; set; }

        public HrRecruitmentUnit? RecruitmentUnit { get; set; }
    }
}
