using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TO.Bkash
{
    [DataContract]
    public class TokenResponse
    {
        [DataMember]
        public string id_token { get; set; }
    }
}
