using LapTrinhWeb.Models.Vnpay;
using System.Collections.Specialized;
using System.Web;

namespace LapTrinhWeb.Services.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContextBase context);
        PaymentResponseModel PaymentExecute(NameValueCollection collections);
    }


}
