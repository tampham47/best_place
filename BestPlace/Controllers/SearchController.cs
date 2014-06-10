using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index(string request, int categoryId = 1)
        {
            ViewBag.CategoryId = categoryId;
            ViewBag.CategoryName = db.bp_Category_GetById(categoryId).Single().Name;
            ViewBag.Request = request;

            return View(db.bp_Place_Search(request, categoryId).ToList());
        }

        public ActionResult SearchMenu(string request="")
        {
            ViewBag.Request = request;
            return View();
        }

        public ActionResult All(string request)
        {
            ViewBag.Request = request;
            return View();
        }

        public ActionResult User(string request = "")
        {
            var profiles = db.bp_Profile_Search(request).ToList();
            ViewBag.AmountOfUser = profiles.Count();
            ViewBag.Request = request;
            return View(profiles);
        }
    }
}
