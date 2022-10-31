using Microsoft.Extensions.DependencyInjection;
using PaymentProviders.Factory;

namespace PaymentProviders
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentProvider(this IServiceCollection services)
        {  

            services.AddTransient<IPaymentProviderFactory, PaymentProviderFactory>();
             
            return services;
        }
    }
}
