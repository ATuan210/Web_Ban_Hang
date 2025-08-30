using LapTrinhWeb.Libraries;
using LapTrinhWeb.Models.Vnpay;
using LapTrinhWeb.Services.Vnpay;
using System;
using System.Web;
using System.Web.Mvc;
using System.Configuration;


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
        var url = _vnPayService.CreatePaymentUrl(model, System.Web.HttpContext.Current);

        if (string.IsNullOrEmpty(url))
        {
            return Content("Không tạo được URL thanh toán VNPAY. Kiểm tra lại service.");
        }

        return Redirect(url);


        // HttpContext trong MVC5: System.Web.HttpContext.Current
        //var url = _vnPayService.CreatePaymentUrl(model, System.Web.HttpContext.Current);

        //return Redirect(url);
        //return Content(url);
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
