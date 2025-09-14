using LapTrinhWeb.Libraries;
using LapTrinhWeb.Models.Vnpay;
using LapTrinhWeb.Services.Vnpay;
using System;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace LapTrinhWeb.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public ActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            if (model == null || model.Amount <= 0)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Tạo URL sang VNPAY
            var paymentUrl = _vnPayService.CreatePaymentUrl(model, this.HttpContext);

            // Redirect sang cổng thanh toán
            return Redirect(paymentUrl);


        }
        


        [HttpGet]
        public ActionResult PaymentCallbackVnpay()
        {
            // Trong MVC5, QueryString lấy qua Request.QueryString
            //var response = _vnPayService.PaymentExecute(Request.QueryString);

            // Trả JSON trong MVC5 dùng JsonResult
            //return Json(response, JsonRequestBehavior.AllowGet);

            var response = _vnPayService.PaymentExecute(Request.QueryString);

            if (response.Success)
            {
                return RedirectToAction("Success", "Checkout", new { orderId = response.OrderId });
            }
            else
            {
                return RedirectToAction("Fail", "Checkout");
            }
        }



    }
}

