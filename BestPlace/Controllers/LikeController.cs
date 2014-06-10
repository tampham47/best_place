using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class LikeController : Controller
    {
        //
        // GET: /Like/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult LikePlace(Guid placeId)
        {
            var userId = (Guid)Membership.GetUser().ProviderUserKey;
            if (db.bp_Like_GetByUserAndObject(placeId, userId).Count() == 0)
            {
                db.bp_Like_Create(placeId, userId);
            }

            return RedirectToAction("Details", "Place", new { placeId = placeId });
        }

    }
}
