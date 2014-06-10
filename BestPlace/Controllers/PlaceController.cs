using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class PlaceController : Controller
    {
        //
        // GET: /Place/
        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index(int categoryId)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = db.bp_Category_GetById(categoryId).Single().Name;
            return View(db.bp_Place_GetByCategory(categoryId).ToList());
        }

        public ActionResult Details(Guid placeId)
        {
            var place = db.bp_Place_GetById(placeId).Single();
            ViewBag.Category = db.bp_Category_GetById(place.CategoryId).Single();
            return View(place);
        }

        [Authorize]
        public ActionResult New(int categoryId)
        {
            ViewBag.CategoryName = db.bp_Category_GetById(categoryId).Single().Name;

            bp_Place place = new bp_Place();
            place.PlaceId = Guid.NewGuid();
            return View(place);
        }

        [HttpPost]
        public ActionResult New(bp_Place place)
        {
            if (ModelState.IsValid && db.bp_Photo_GetByPlace(place.PlaceId).Count() > 0)
            {
                db.bp_Place_Create(
                    place.PlaceId,
                    (Guid)Membership.GetUser().ProviderUserKey,
                    place.CategoryId,
                    place.Title,
                    place.Latitude,
                    place.Longitude,
                    place.Descript,
                    place.PhoneNumber,
                    place.Address,
                    place.WebSite);

                return RedirectToAction("Details", "Place", new { placeId = place.PlaceId });
            }
            else
            {
                place.Descript = HttpUtility.HtmlDecode(place.Descript);
                return View(place);
            }
        }

        [Authorize]
        public ActionResult Edit(Guid placeId)
        {
            if (db.bp_Place_GetById(placeId).Single().UserId == (Guid)Membership.GetUser().ProviderUserKey)
            {
                bp_Place place = db.bp_Place_GetById(placeId).Single();
                ViewBag.Category = db.bp_Category_GetById(place.CategoryId).Single();
                place.Descript = HttpUtility.HtmlDecode(place.Descript);

                return View(place);
            }
            else
                return RedirectToAction("Details", "Place", new { placeId = placeId });
        }

        [HttpPost]
        public ActionResult Edit(bp_Place place)
        {
            if (ModelState.IsValid)
            {
                db.bp_Place_Update(
                    place.PlaceId,
                    place.CategoryId,
                    place.Title,
                    place.Latitude,
                    place.Longitude,
                    place.Descript,
                    place.PhoneNumber,
                    place.Address,
                    place.WebSite);

                db.bp_Place_UpdateState(place.PlaceId, (int)PlaceState.Waitting);

                return RedirectToAction("Details", new { placeId = place.PlaceId });
            }
            else
                return View(place);
        }

        public ActionResult Delete(Guid placeId)
        {
            if (db.bp_Place_GetById(placeId).Single().UserId == (Guid)Membership.GetUser().ProviderUserKey)
            {
                var categoryId = db.bp_Place_GetById(placeId).Single().CategoryId;

                db.bp_Place_Delete(placeId);
                
                return RedirectToAction("Index", "Place", new { categoryId = categoryId });
            }
            else
                return RedirectToAction("Details", "Place", new { placeId = placeId });
        }

        public ActionResult Photos(Guid placeId)
        {
            ViewBag.PlaceId = placeId;
            var photos = db.bp_Photo_GetByPlace(placeId).ToList();
            return View(photos);
        }


        //get all places by user
        public ActionResult ByUser(Guid? userId)
        {
            if (!userId.HasValue)
            {
                userId = (Guid)Membership.GetUser().ProviderUserKey;
            }

            ViewBag.UserId = userId;
            ViewBag.UserName = db.bp_Profile_GetById(userId).Single().UserName;
            var places = db.bp_Place_GetByUser(userId).ToList();

            return View(places);
        }
    }
}