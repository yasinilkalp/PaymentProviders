using Microsoft.AspNetCore.Http;
using PaymentProviders.Models;
using PaymentProviders.Models.SafeKey;
using System.Threading.Tasks;

namespace PaymentProviders
{
    public interface IPaymentProvider
    {
        PaymentParameterResult GetPaymentParameters(PaymentRequest request);
        PaymentResult GetPaymentResult(IFormCollection form);

        Task<PaymentResponse> CreateSafeKey(CreateSafeKeyRequest request);
        Task<PaymentResponse> DisableSafeKey(DisableSafeKeyRequest request);
        Task<PaymentResponse<SafeKeyResponse>> GetSafeKey(ListSafeKeyRequest request);
    }
}
