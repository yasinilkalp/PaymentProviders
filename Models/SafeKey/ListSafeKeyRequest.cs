using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace PaymentProviders.Models.SafeKey
{
    [XmlRoot("CC5Request")]
    public class ListSafeKeyRequest : SafeKeyAuth
    {  
        public CreateSafeKeyExtra Extra { get; set; }

        public class CreateSafeKeyExtra
        {   
            public string MERCHANTSAFE { get; set; } = "GETPANS";
            public string MERCHANTSAFEKEY { get; set; } 

        }
    }  

    public class SafeKeyResponse
    {  
        public string NUMBEROFPANS { get; set; }
        public string PAN1 { get; set; }
        public string PAN1LABEL { get; set; }
        public string PAN1STATUS { get; set; }
        public string PAN1INDEXORDER { get; set; }
        public string PAN1OWNER { get; set; }
        public string PAN1EXPIRY { get; set; }
        public string PAN1DESCRIPTION { get; set; }
    }
}
