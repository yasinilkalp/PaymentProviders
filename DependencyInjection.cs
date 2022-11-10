using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentProviders.Factory;
using PaymentProviders.Services;
using System;

namespace PaymentProviders
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentProvider(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddTransient<IPaymentProviderFactory, PaymentProviderFactory>();

            string ApiUrl = configuration.GetSection("PaymentProviderBaseUrl:IsBankasiBaseUrl").Value;
            services.AddHttpClient<IPaymentRequestService, PaymentRequestService>(c => c.BaseAddress = new Uri(ApiUrl));

            return services;
        }
    }
}
