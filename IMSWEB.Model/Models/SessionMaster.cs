using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    [Table("SessionMasters")]
    public partial class SessionMaster

    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SessionMaster()
        {
            UserAuditDetails = new HashSet<UserAuditDetail>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public DateTime SessionStartDT { get; set; }
        public DateTime? SessionEndDT { get; set; }
        public string IPAddress { get; set; }
        public int ConcernID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAuditDetail> UserAuditDetails { get; set; }
    }
}
