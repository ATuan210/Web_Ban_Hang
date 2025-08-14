using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LapTrinhWeb.Models;
using System.Net;
using System.Diagnostics.CodeAnalysis;


namespace DoAnLapTrinhWeb.Controllers
{
    public class ShoppingCartController : Controller
    {
        private Product_DBContext dBContext = new Product_DBContext();
        private string strCart = "Cart";
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderNow(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session[strCart] == null)
            {
                List<Cart> ListCart = new List<Cart>
                {
                    new Cart(dBContext.Products.Find(Id),1)
                };
                Session[strCart] = ListCart;
            }
            else
            {
                List<Cart> ListCart = (List<Cart>)Session[strCart];
                int check = IsExistingCheck(Id);
                if (check == -1)
                    ListCart.Add(new Cart(dBContext.Products.Find(Id), 1));
                else
                    ListCart[check].Quantity++;
                Session[strCart] = ListCart;
            }
            return RedirectToAction("Index");
        }

        private int IsExistingCheck(int? Id)
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                if (ListCart[i].Product.ProId == Id)
                    return i;
            }
            return -1;
        }

        public ActionResult RemoveItem(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int check = IsExistingCheck(Id);
            List<Cart> ListCard = (List<Cart>)Session[strCart];
            ListCard.RemoveAt(check);
            if (ListCard.Count == 0)
            {
                Session[strCart] = null;
            }
            else
            {
                Session[strCart] = ListCard;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateCart(FormCollection field)
        {
            string[] quantities = field.GetValues("quantity");
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                ListCart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            Session[strCart] = ListCart;
            return RedirectToAction("Index");
        }

        public ActionResult ClearCart()
        {
            Session[strCart] = null;
            return RedirectToAction("Index");
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProcessOrder(FormCollection field)
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];

            //1. Save the order into Order table
            var order = new LapTrinhWeb.Models.Order()
            {
                CustomerName = field["cusName"],
                CustomerPhone = field["cusPhone"],
                CustomerEmail = field["cusEmail"],
                CustomerAddress = field["cusAddress"],
                OrderDate = DateTime.Now,
                PaymentType = "Cash",
                Status = "Processing"
            };
            dBContext.Orders.Add(order);
            dBContext.SaveChanges();

            //2. Save the order details into Order Details table
            foreach (Cart cart in ListCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.OrderId,
                    ProductId = cart.Product.ProId,
                    Quantity = Convert.ToInt32(cart.Quantity),
                    Price = Convert.ToDouble(cart.Product.ProPrice)
                };
                dBContext.OrderDetails.Add(orderDetail);
                dBContext.SaveChanges();
            }

            //3. Remove shopping cart session
            Session.Remove(strCart);
            return View("OrderSuccess");
        }
    }
}