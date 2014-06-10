using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BestPlace.Models;
using BestPlaceManager.Models;

namespace BestPlaceManager.Controllers
{
    public class PlaceManagerController : Controller
    {
        //
        // GET: /PlaceManager/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index()
        {
            var places = db.bp_Place_GetAll().ToList();
            return View(places);
        }

        public ActionResult Details(Guid placeId)
        {
            return View();
        }

        public ActionResult Denied(Guid placeId)
        {
            db.bp_Place_UpdateState(placeId, (int)PlaceState.Denied);
            return RedirectToAction("Index");
        }

        public ActionResult Actived(Guid placeId)
        {
            db.bp_Place_UpdateState(placeId, (int)PlaceState.Actived);
            return RedirectToAction("Index");
        }
    }
}
