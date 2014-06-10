using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class ArrivedController : Controller
    {
        //
        // GET: /Arrived/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index(Guid? userId)
        {
            if (!userId.HasValue)
                userId = (Guid)Membership.GetUser().ProviderUserKey;

            ViewBag.UserName = db.bp_Profile_GetById(userId).Single().UserName;
            ViewBag.UserId = userId;

            return View(db.bp_Place_GetByArrived(userId).ToList());
        }

        [Authorize]
        public ActionResult Add(Guid placeId)
        {
            if (db.bp_Arrived_GetByUserPlace((Guid)Membership.GetUser().ProviderUserKey, placeId).Count() == 0)
            {
                db.bp_Arrived_Create((Guid)Membership.GetUser().ProviderUserKey, placeId);
            }

            return RedirectToAction("Details", "Place", new { placeId = placeId });
        }
    }
}
