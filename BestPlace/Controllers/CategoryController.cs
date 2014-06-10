using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/
        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index()
        {
            return View(db.bp_Category.ToList());
        }

    }
}
