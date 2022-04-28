using LinkedHU_CENG.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Http;



namespace LinkedHU_CENG.Controllers
{

    public class UserController : Controller
    {
        private readonly ApplicationDbContext db;

        public UserController(ApplicationDbContext context)
        {
            this.db = context;
        }

        public IActionResult Index()
        {
            return View(db.Users.ToList());
        }



        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserID") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        public IActionResult Login(User usr)
        {
            
            var info = db.Users.FirstOrDefault(u => u.Email.Equals(usr.Email) && u.Password.Equals(usr.Password));
            if (info != null)
            {
                HttpContext.Session.SetInt32("UserID", info.UserId);
                HttpContext.Session.SetString("Email", info.Email);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserID") != null)
            {
                //HttpContext.Session.Clear();
                HttpContext.Session.Remove("UserID");
                HttpContext.Session.Remove("Email");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
