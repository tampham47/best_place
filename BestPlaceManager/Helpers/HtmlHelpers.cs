using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Facebook;
using BestPlace.Models;


namespace BestPlaceManager.Helpers
{
    public static class HtmlHelpers
    {
        public static string Truncate(this HtmlHelper help, string content, int length)
        {
            content = content.Replace("&nbsp;", " ");
            int f = content.IndexOf('<'), l = content.IndexOf('>');
            while (f >= 0 && f < l)
            {
                content = content.Remove(f, l - f + 1);
                f = content.IndexOf('<');
                l = content.IndexOf('>');
            }

            if (content.Length < length)
                return content;
            else
                return content.Substring(0, length) + "...";
        }

        public static bp_Profile GetProfile(this HtmlHelper help, Guid? userId)
        {
            if (!userId.HasValue)
                userId = (Guid)Membership.GetUser().ProviderUserKey;

            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                return db.bp_Profile_GetById(userId).Single();
            }
        }

        public static bp_Category GetCategory(this HtmlHelper help, int categoryId)
        {
            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                return db.bp_Category_GetById(categoryId).Single();
            }
        }

        public static List<bp_Photo> Photos(this HtmlHelper help, Guid placeId)
        {
            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                return db.bp_Photo_GetByPlace(placeId).ToList();
            }
        }

        public static int AmountOfLike(this HtmlHelper help, Guid objectId)
        {
            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                return db.bp_Like_GetByObject(objectId).Count();
            }
        }

        public static bool IsLike(this HtmlHelper help, Guid userId, Guid objectId)
        {
            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                if (db.bp_Like_GetByUserAndObject(objectId, userId).Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public static bool HasArrived(this HtmlHelper help, Guid userId, Guid placeId)
        {
            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                if (db.bp_Arrived_GetByUserPlace(userId, placeId).Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public static int AmountOfArrived(this HtmlHelper help, Guid placeId)
        {
            using (BestPlaceEntities db = new BestPlaceEntities())
            {
                return db.bp_Arrived_GetByPlaceId(placeId).Count();
            }
        }
    }
}