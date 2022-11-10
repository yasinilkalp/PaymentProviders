using System.Xml.Serialization;

namespace PaymentProviders.Models.SafeKey
{
    [XmlRoot("CC5Request")]
    public class CreateSafeKeyRequest : SafeKeyAuth
    {
        public CreateSafeKeyRequest()
        {
            Extra = new();
        }
        public string Number { get; set; }
        public string Expires { get; set; }
        public CreateSafeKeyExtra Extra { get; set; }

        public class CreateSafeKeyExtra
        {   
            public string MERCHANTSAFE { get; set; } = "ADDPAN";
            public string MERCHANTSAFEKEY { get; set; }
            public string MERCHANTSAFECARDOWNER { get; set; }
            public string MERCHANTSAFELABEL { get; set; }
            public string MERCHANTSAFEDESCRIPTION { get; set; }

        }
    }  
}
