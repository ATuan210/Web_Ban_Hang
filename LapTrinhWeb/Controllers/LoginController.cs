using LapTrinhWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;


namespace LapTrinhWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Accounts
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel model)
        {
            using (Product_DBContext context = new Product_DBContext())
            {
                model.UserPassword = GetMD5(model.UserPassword);
                bool isValidUser = context.Users.Any(user => user.UserName.ToLower() ==
                    model.UserName.ToLower() && user.UserPassword == model.UserPassword);
                if (isValidUser)
                {
                    // Lấy thông tin user và role
                    var user = context.UserRolesMappings
                        .Include(urm => urm.User)
                        .Include(urm => urm.RoleMaster)
                        .FirstOrDefault(urm => urm.User.UserName == model.UserName);
                    Session["UserName"] = model.UserName;
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    // Lấy roleName từ DB
                    var roleName = user.RoleMaster.RoleName;

                    // Tạo ticket kèm role
                    var authTicket = new FormsAuthenticationTicket(
                        1,
                        model.UserName,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        false,
                        roleName
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    Response.Cookies.Add(authCookie);
                    
                    if (roleName == "Admin")
                    {
                        return RedirectToAction("Index", "Products", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Products", new { area = "Admin" });
                    }
                }
                ModelState.AddModelError("", "Invalid Username or Password");
            }
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Signup(UserModel userModel)
        {
            using (Product_DBContext context = new Product_DBContext())
            {
                // Map từ UserModel sang User (entity)
                var user = new User
                {
                    UserName = userModel.UserName,
                    UserPassword = GetMD5(userModel.UserPassword),
                };

                // Lưu User vào DB
                context.Users.Add(user);
                context.SaveChanges();

                // Lấy role mặc định
                var defaultRole = context.RoleMasters.FirstOrDefault(r => r.RoleName == "User");

                if (defaultRole != null)
                {
                    // Gán role cho user vừa đăng ký
                    var mapping = new UserRolesMapping
                    {
                        UserID = user.ID,         // ID đã có sau SaveChanges
                        RoleID = defaultRole.ID
                    };

                    context.UserRolesMappings.Add(mapping);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
        public static string GETSHA256(string str)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = sha256.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
    }
}
