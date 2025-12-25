using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SaleOffer : IModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OfferID { get; set; }

        [StringLength(250)]
        public string OfferCode { get; set; }

        public int ProductID { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        public decimal OfferValue { get; set; }

        public EnumSalesOfferType OfferType { get; set; }

        public decimal ThresholdValue { get; set; }

        public EnumOfferStatus Status { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Product Product { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }
    }
}
