using LapTrinhWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;

namespace LapTrinhWeb.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        private Product_DBContext dBContext = new Product_DBContext();
        public ActionResult Index(int? categoryId, int? page)
        {
            int pageSize = 9;
            int pageNumber = page ?? 1;

            // Lấy d/s sản phẩm từ database
            IQueryable<Product> productsQuery = dBContext.Products.Include(p => p.Category);



            // Lọc theo categoryId nếu được truyền
            if (categoryId.HasValue && categoryId > 0)
            {
                productsQuery = productsQuery.Where(p => p.CatId == categoryId.Value);
            }

            // Sắp xếp d/s các sản phẩm
            productsQuery = productsQuery.OrderBy(p => p.ProName);

            // Lưu categoryId vào ViewBag để giữ filter khi phân trang
            ViewBag.CurrentCategoryId = categoryId;

            // Áp dụng phân trang
            var pagedProducts = productsQuery.ToPagedList(pageNumber, pageSize);

            // Truyền danh sách phân trang sang View
            return View(pagedProducts);
                
        }

        public ActionResult Details(int id)
        {
            Product_DBContext dBContext = new Product_DBContext();
            Product product = dBContext.Products.Include(cat => cat.Category).FirstOrDefault(x => x.ProId == id);
            return View(product);
        }
    }
}