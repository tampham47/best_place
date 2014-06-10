using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class HomeController : Controller
    {
        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View(db.bp_Category_GetAll().ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        [Authorize]
        public ActionResult ProfileInfo()
        {
            var facebookId = long.Parse(User.Identity.Name);
            var user = InMemoryUserStore.Get(facebookId);
            var client = new FacebookClient(user.AccessToken);

            dynamic me = client.Get("me?fields=id,name,username,email,birthday");
            ViewBag.Name = me.name;
            ViewBag.Id = me.id;
            ViewBag.Email = me.email;
            ViewBag.Birthday = me.birthday;
            ViewBag.UserId = Membership.GetUser().ProviderUserKey;
            return View();
        }

        public ActionResult UserMenu(Guid userId)
        {
            return View(userId);
        }
    }
}
