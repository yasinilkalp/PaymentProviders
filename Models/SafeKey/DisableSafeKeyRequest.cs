using System.Xml.Serialization;

namespace PaymentProviders.Models.SafeKey
{
    [XmlRoot("CC5Request")]
    public class DisableSafeKeyRequest : SafeKeyAuth
    { 
        public DisableSafeKeyExtra Extra { get; set; }

        public class DisableSafeKeyExtra
        {   
            public string MERCHANTSAFE { get; set; } = "DISABLEPANS";
            public string MERCHANTSAFEKEY { get; set; }  
        }
    }
}
