using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class OldUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [StringLength(150)]
        public string UserName { get; set; }

        [Required]
        [StringLength(250)]
        public string UserPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactNo { get; set; }

        [StringLength(100)]
        public string EmailAddress { get; set; }

        public int UserType { get; set; }

        public DateTime CreateDate { get; set; }

        public int CreatedBy { get; set; }

        public int Status { get; set; }

        public int ConcernID { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
    }
}
