using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class Grade : AuditTrailModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Grade()
        {
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GradeID { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(250)]
        public string Code { get; set; }
        public int UserID { get; set; }
        public int PayrollTypeID { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }
        public int HasGrossConcept { get; set; }
        public decimal BasicPercentOfGross { get; set; }
    }
}
