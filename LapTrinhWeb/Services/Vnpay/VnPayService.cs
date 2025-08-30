using LapTrinhWeb.Libraries;
using LapTrinhWeb.Models.Vnpay;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;

namespace LapTrinhWeb.Services.Vnpay
{
    public class VnPayService : IVnPayService
    {
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = ConfigurationManager.AppSettings["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", ConfigurationManager.AppSettings["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", ConfigurationManager.AppSettings["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", ConfigurationManager.AppSettings["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", ConfigurationManager.AppSettings["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
              pay.CreateRequestUrl(ConfigurationManager.AppSettings["Vnpay:BaseUrl"], ConfigurationManager.AppSettings["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(NameValueCollection queryString)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(queryString, ConfigurationManager.AppSettings["Vnpay:HashSecret"]);
            return response;
        }
    }
}
