using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class PhotoController : Controller
    {
        //Database
        BestPlaceEntities db = new BestPlaceEntities();

        //
        // GET: /Photo/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(Guid photoId)
        {
            var photo = db.bp_Photo_GetById(photoId).Single();
            return View(photo);
        }

        [HttpPost]
        public ActionResult Edit(bp_Photo photo)
        {
            if (ModelState.IsValid)
            {
                //db.bp_pho
            }
            return View(photo);
        }
    }
}
