using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    [Table("UserAuditDetails")]
    public partial class UserAuditDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int AuditID { get; set; }
        public int SessionID { get; set; }
        public int ObjectID { get; set; }
        public EnumObjectType ObjectType { get; set; }
        public int ActionPerformedRole { get; set; }
        public EnumActionType ActionType { get; set; }
        public DateTime ActivityDtTime { get; set; }
        public string ActivityURL { get; set; }
        public int ConcernID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public virtual SessionMaster SessionMaster { get; set; }
    }
}
