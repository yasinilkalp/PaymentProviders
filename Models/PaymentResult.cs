using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PaymentProviders.Models
{


    public class PaymentResult
    {


        public bool Success { get; set; }
        public string ErrorCode { get; set; }


        public string TransactionNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string CardHolderName { get; set; }
        public string MaskedCreditCard { get; set; }
        public int Installment { get; set; }
        public decimal TotalAmount { get; set; }
        public string BankErrorMessage { get; set; }
        public DateTimeOffset? PaidDate { get; set; }
        public string ErrMsg { get; set; }
        public string Status { get; set; }
        public string MdStatus { get; set; }
        public string StatusText
        {
            get
            {
                if (MdStatus == "1") return ErrMsg;
                return int.TryParse(MdStatus, out _) ? statuses[int.Parse(MdStatus)] : "";
            }
        }
        public string BankRequest { get; set; }

        public string CardIssuer { get; set; }
        public string CardBrand { get; set; }

        public static PaymentResult Error(PaymentResult result)
        {
            result.Success = false;
            return result;
        }

        public static string GenerateBankRequest(IFormCollection form)
        {
            Dictionary<string, string> values = new();

            foreach (string item in form.Keys) values.Add(item, form[item]);

            return JsonConvert.SerializeObject(values);
        }

        public static DateTimeOffset? ConvertDate(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;

            return DateTime.ParseExact(value, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private Dictionary<int, string> statuses => new()
        {
            { 0 , "3-D Secure imzası geçersiz veya doğrulama" },
            { 1 , "İşlem Başarılı" },
            { 2 , "Kart sahibi veya bankası sisteme kayıtlı değil" },
            { 3 , "Kartın bankası sisteme kayıtlı değil" },
            { 4 , "Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş" },
            { 5 , "Doğrulama yapılamıyor" },
            { 6 , "3-D Secure hatası" },
            { 7 , "Sistem hatası" },
            { 8 , "Bilinmeyen kart no" },
        };

        public static bool FailStatusControl(string status, string[] controlStatuses) => !controlStatuses.Contains(status);

    }


}
