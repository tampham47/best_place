using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BestPlace.Models;

namespace BestPlace.Controllers
{
    public class CommentController : Controller
    {
        //
        // GET: /Comment/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetByPlace(Guid placeId)
        {
            ViewBag.PlaceId = placeId;
            return View(db.bp_Comment_GetByPlace(placeId).ToList());
        }

        public ActionResult New(Guid placeId)
        {
            bp_Comment comment = new bp_Comment();
            comment.PlaceId = placeId;
            comment.UserId = (Guid)Membership.GetUser().ProviderUserKey;

            return View(comment);
        }

        [HttpPost]
        public ActionResult New(bp_Comment comment)
        {
            if (ModelState.IsValid)
            {
                System.Data.Objects.ObjectParameter commentId = new System.Data.Objects.ObjectParameter("commentId", Guid.Empty);
                db.bp_Comment_Create(
                    commentId,
                    (Guid)Membership.GetUser().ProviderUserKey,
                    comment.PlaceId,
                    comment.Content);

                return RedirectToAction("Details", "Place", new { placeId = comment.PlaceId});
            }
            else
                return View(comment);
        }

    }
}
