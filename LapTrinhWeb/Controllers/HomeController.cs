using LapTrinhWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapTrinhWeb.Controllers
{
    public class HomeController : Controller
    {
        Product_DBContext dBContext = new Product_DBContext();
        public ActionResult Index(int ? id)
        {
            var products = dBContext.Products
                                .Include(cat => cat.Category)  
                                .Where(x => x.ProId != id)    
                                .Take(6)                     
                                .ToList();

            return View(products);  
        }

        public ActionResult Details(int id)
        {
            Product_DBContext dBContext = new Product_DBContext();
            Product product = dBContext.Products.Include(cat => cat.Category).FirstOrDefault(x => x.ProId == id);
            return View(product);
        }

        

    }
}