using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class AllowanceDeduction:AuditTrailModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AllowanceDeduction()
        {
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllowDeductID { get; set; }

        [Required]
        [StringLength(500)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Code { get; set; }
        public int AllowORDeduct { get; set; }
        public int SequenceNO { get; set; }
        public int Status { get; set; }


        //public int ConcernID { get; set; }

        //[ForeignKey("ConcernID")]
        //public virtual SisterConcern SisterConcern { get; set; }
    }
}
