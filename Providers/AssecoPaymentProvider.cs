using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PaymentProviders.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace PaymentProviders.Providers
{
    public class AssecoPaymentProvider : IPaymentProvider
    {

        public PaymentParameterResult GetPaymentParameters(PaymentRequest request)
        {

            string PaymentUrl = request.BankUrl; //"https://entegrasyon.asseco-see.com.tr/fim/est3Dgate"
            string clientId = request.ClientId; // "700655000200";//Mağaza numarası
            string storeKey = request.StoreKey; // "TRPS1234";//Mağaza anahtarı
            string processType = "Auth";//İşlem tipi
            string storeType = "3D_PAY";//SMS onaylı ödeme modeli 3DPay olarak adlandırılıyor.
            string successUrl = request.SuccessUrl;//Başarılı Url
            string failUrl = request.FailUrl;//Hata Url
            string random = DateTime.Now.ToString();


            string cardType = "1"; //Kart Ailesi Visa 1 | MasterCard 2 | Amex 3
            if (request.CardNumber.Substring(0, 1) == "4")
                cardType = "1";
            else if (request.CardNumber.Substring(0, 1) == "5")
                cardType = "2";
            else if (request.CardNumber.Substring(0, 1) == "6")
                cardType = "3";
            else
                cardType = "";

            var parameterResult = new PaymentParameterResult();
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters.Add("clientid", clientId);
                parameters.Add("amount", request.TotalAmount.ToString(new CultureInfo("en-US")));//kuruş ayrımı nokta olmalı!!!
                parameters.Add("oid", request.OrderNumber);//sipariş numarası

                //işlem başarılı da olsa başarısız da olsa callback sayfasına yönlendirerek kendi tarafımızda işlem sonucunu kontrol ediyoruz
                parameters.Add("okUrl", successUrl);//başarılı dönüş adresi
                parameters.Add("failUrl", failUrl);//hatalı dönüş adresi
                parameters.Add("islemtipi", processType);//direk satış
                parameters.Add("taksit", request.Installment);//taksit sayısı | 1 veya boş tek çekim olur
                parameters.Add("rnd", random);//rastgele bir sayı üretilmesi isteniyor

                string hashstr = clientId + request.OrderNumber + request.TotalAmount + successUrl + failUrl + processType + request.Installment + random + storeKey;
                var cryptoServiceProvider = SHA1.Create();
                var inputbytes = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(hashstr));
                var hashData = Convert.ToBase64String(inputbytes);

                parameters.Add("hash", hashData);//hash data
                parameters.Add("currency", request.CurrencyIsoCode);//TL ISO code | EURO 978 | Dolar 840

                //kart numarasından çizgi ve boşlukları kaldırıyoruz
                string cardNumber = request.CardNumber.Replace("-", string.Empty);
                cardNumber = cardNumber.Replace(" ", string.Empty).Trim();
                parameters.Add("pan", cardNumber);

                parameters.Add("cardHolderName", request.CardHolderName);
                parameters.Add("Ecom_Payment_Card_ExpDate_Month", request.ExpireMonth);//kart bitiş ay'ı
                parameters.Add("Ecom_Payment_Card_ExpDate_Year", request.ExpireYear);//kart bitiş yıl'ı
                parameters.Add("cv2", request.CvvCode);//kart güvenlik kodu
                parameters.Add("cardType", cardType);//kart tipi visa 1 | master 2 | amex 3
                parameters.Add("storetype", storeType);
                parameters.Add("lang", request.LanguageIsoCode);//iki haneli dil iso kodu

                parameterResult.Parameters = parameters;
                parameterResult.Success = true;

                //İş Bankası Canlı https://sanalpos.isbank.com.tr/fim/est3Dgate
                parameterResult.PaymentUrl = new Uri(PaymentUrl);
            }
            catch (Exception ex)
            {
                parameterResult.Success = false;
                parameterResult.ErrorMessage = ex.ToString();
            }

            return parameterResult;
        }

        public PaymentResult GetPaymentResult(IFormCollection form)
        {
            //
            if (form == null) return new PaymentResult() { BankErrorMessage = "Form verisi alınamadı." };

            PaymentResult result = new()
            {
                Success = true,
                MdStatus = form["mdStatus"],
                Status = form["Response"],
                TransactionNumber = form["TransId"],
                ReferenceNumber = form["oid"],
                BankErrorMessage = $"{form["Response"]} - {form["ErrMsg"]} - {form["mdErrorMsg"]}",
                ErrMsg = form["ErrMsg"],
                ErrorCode = form["ProcReturnCode"],
                CardHolderName = form["cardHolderName"],
                Installment = int.TryParse(form["taksit"], out _) ? int.Parse(form["taksit"]) : 1,
                MaskedCreditCard = form["maskedCreditCard"],
                TotalAmount = decimal.Parse(form["amount"], new CultureInfo("en-US")),
                CardBrand = form["EXTRA.CARDBRAND"],
                PaidDate = PaymentResult.ConvertDate(form["EXTRA.TRXDATE"]),
                CardIssuer = form["EXTRA.CARDISSUER"],
                BankRequest = PaymentResult.GenerateBankRequest(form),
            };

            // mdstatus 1,2,3 veya 4 olursa 3D doğrulama geçildi anlamına geliyor 
            if (PaymentResult.FailStatusControl(result.MdStatus, new string[] { "1", "2", "3", "4" })) return PaymentResult.Error(result);
             
            if (StringValues.IsNullOrEmpty(result.Status) || !result.Status.Equals("Approved"))
                return PaymentResult.Error(result);

            return result;
        }
    }
}