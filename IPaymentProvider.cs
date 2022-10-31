using Microsoft.AspNetCore.Http;
using PaymentProviders.Models;

namespace PaymentProviders
{
    public interface IPaymentProvider
    {
        PaymentParameterResult GetPaymentParameters(PaymentRequest request);
        PaymentResult GetPaymentResult(IFormCollection form);
    }
}
