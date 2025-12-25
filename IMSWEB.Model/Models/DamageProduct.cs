using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class DamageProduct : IModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DamageProID { get; set; }

        public DateTime EntryDate { get; set; }

        public int ProductID { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
    }
}
