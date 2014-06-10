using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index(Guid? userId)
        {
            if (!userId.HasValue)
                userId = (Guid)Membership.GetUser().ProviderUserKey;

            ViewBag.UserId = userId;
            var profile = BestPlace.Helpers.HtmlHelpers.GetProfile(null, userId);

            return View(profile);
        }

        public ActionResult Search(string str = "")
        {
            var profiles = db.bp_Profile_Search(str).ToList();
            ViewBag.AmountOfUser = profiles.Count();
            ViewBag.Request = str;
            return View(profiles);
        }

        [Authorize]
        public ActionResult Update()
        {
            var profile = db.bp_Profile_GetById((Guid)Membership.GetUser().ProviderUserKey).Single();
            //var profile = BestPlace.Helpers.HtmlHelpers.GetProfile(null, (Guid)Membership.GetUser().ProviderUserKey);
            return View(profile);
        }

        [HttpPost]
        public ActionResult Update(bp_Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.bp_Profile_Update(
                    profile.UserId,
                    profile.UserName,
                    profile.Email,
                    profile.PhoneNumber,
                    profile.Birthday,
                    profile.Gender,
                    null, null);

                return RedirectToAction("Index");
            }

            return View(profile);
        }
    }
}
