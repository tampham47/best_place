using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BestPlace.Models;

namespace BestPlaceManager.Controllers
{
    public class UserManagerController : Controller
    {
        //
        // GET: /UserManager/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index(string requestStr = "")
        {
            var profiles = db.bp_Profile_Search(requestStr).ToList();
            return View(profiles);
        }

    }
}
