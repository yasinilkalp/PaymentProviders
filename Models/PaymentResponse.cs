using System.Xml.Serialization;

namespace PaymentProviders.Models
{
    [XmlRoot("CC5Response")]
    public class PaymentResponse
    {
        public string Response { get; set; }
        public string ProcReturnCode { get; set; }
        public string ErrMsg { get; set; } 

    }

    [XmlRoot("CC5Response")]
    public class PaymentResponse<T> : PaymentResponse
    {   
        public T Extra { get; set; } 
    }
     
}
