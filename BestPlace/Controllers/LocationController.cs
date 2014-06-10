using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class LocationController : Controller
    {
        //
        // GET: /Location/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index(Guid? userId)
        {
            if (!userId.HasValue)
                userId = (Guid)Membership.GetUser().ProviderUserKey;

            ViewBag.UserName = db.bp_Profile_GetById(userId).Single().UserName;
            ViewBag.UserId = userId;

            return View(db.bp_Location_GetByUser(userId).ToList());
        }

        [Authorize]
        public ActionResult New()
        {
            bp_Location location = new bp_Location();
            return View(location);
        }

        [HttpPost]
        public ActionResult New(bp_Location location)
        {
            if (ModelState.IsValid)
            {
                System.Data.Objects.ObjectParameter locationId = new System.Data.Objects.ObjectParameter("locationId", Guid.Empty);
                db.bp_Location_Create(
                    locationId,
                    (Guid)Membership.GetUser().ProviderUserKey,
                    location.Latitude,
                    location.Longitude,
                    location.Address);

                return RedirectToAction("Index");
            }
            else
                return View(location);
        }

        public ActionResult SetCurrentLocation(Guid locationId)
        {
            var location = db.bp_Location_GetById(locationId).Single();
            if (location != null && location.UserId == (Guid)Membership.GetUser().ProviderUserKey)
            {
                db.bp_Location_Update(locationId);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid locationId)
        {
            var location = db.bp_Location_GetById(locationId).Single();
            if (location != null && location.UserId == (Guid)Membership.GetUser().ProviderUserKey)
            {
                db.bp_Location_Delete(locationId);
            }
            return RedirectToAction("Index");
        }

    }
}
