using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using BestPlace.Models;
using BestPlace.Helpers;
using System.Web.Security;

namespace BestPlace.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        BestPlaceEntities db = new BestPlaceEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> attachments, Guid placeId)
        {
            // The Name of the Upload component is "attachments" 
            foreach (var file in attachments)
            {
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var serverName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/Content/CoverImages"), serverName);

                // The files are not actually saved in this demo
                file.SaveAs(physicalPath);

                System.Data.Objects.ObjectParameter photoId = new System.Data.Objects.ObjectParameter("photoId", Guid.Empty);
                db.bp_Photo_Create(photoId,
                    placeId,
                    (Guid)Membership.GetUser().ProviderUserKey,
                    serverName,
                    fileName,
                    "");
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames, Guid placeId)
        {
            // The parameter of the Remove action must be called "fileNames"
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(Server.MapPath("~/Content/CoverImages"), fileName);

                // TODO: Verify user permissions
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo

                    //System.IO.File.Delete(physicalPath);
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Profile_UploadAvatar(IEnumerable<HttpPostedFileBase> avatarAttachments, Guid userId)
        {
            foreach (var file in avatarAttachments)
            {
                // Some browsers send file names with full path. This needs to be stripped.
                string fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                var tempName = "t-" + fileName;
                var serverName = "avt-" + fileName;
                var tempPath = Path.Combine(Server.MapPath("~/Content/CoverImages"), tempName);
                var serverPath = Path.Combine(Server.MapPath("~/Content/CoverImages"), serverName);

                // The files are not actually saved in this demo
                file.SaveAs(tempPath);
                ImageHelper.CropImage(tempPath, serverPath, 300, 300);

                //change avatar image
                db.bp_Profile_UpdateAvatar(
                    userId,
                    serverName);
            }
            // Return an empty string to signify success
            return Content("");
        }

    }
}
