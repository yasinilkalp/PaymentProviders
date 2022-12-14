using Microsoft.Extensions.DependencyInjection;
using PaymentProviders.Models.Enums;
using PaymentProviders.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentProviders.Factory
{
    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentProvider Create(PosEngineType type)
        {
            switch (type)
            {
                case PosEngineType.ASSECO:
                    return ActivatorUtilities.GetServiceOrCreateInstance<AssecoPaymentProvider>(_serviceProvider);

                default:
                    throw new NotSupportedException("Bank engine not supported");
            }
        }

        public string CreatePaymentForm(IDictionary<string, object> parameters, Uri paymentUrl, bool appendSubmitScript = true)
        {
            if (parameters == null || !parameters.Any())
                throw new ArgumentNullException(nameof(parameters));

            if (paymentUrl == null)
                throw new ArgumentNullException(nameof(paymentUrl));

            var formId = "PaymentForm";
            var formBuilder = new StringBuilder();
            formBuilder.Append($"<form id=\"{formId}\" name=\"{formId}\" action=\"{paymentUrl}\" role=\"form\" method=\"POST\">");
            foreach (var parameter in parameters)
            {
                formBuilder.Append($"<input type=\"hidden\" name=\"{parameter.Key}\" value=\"{parameter.Value}\">");
            }
            formBuilder.Append("</form>");

            if (appendSubmitScript)
            {
                var scriptBuilder = new StringBuilder();
                scriptBuilder.Append("<script>");
                scriptBuilder.Append($"document.{formId}.submit();");
                scriptBuilder.Append("</script>");
                formBuilder.Append(scriptBuilder.ToString());
            }

            return formBuilder.ToString();
        }
    }
}
