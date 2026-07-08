using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlignHR.Models
{
    [Table("LeaveYearLocks", Schema = "dbo")]
    public class LeaveYearLock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Year { get; set; }
        public DateTime LockedAt { get; set; }
        public int LockedBy { get; set; }
    }
}
