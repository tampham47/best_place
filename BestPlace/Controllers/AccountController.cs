using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using BestPlace.Models;
using System.Web.Security;
using System.Globalization;

namespace BestPlace.Controllers
{
    public class AccountController : Controller
    {
        BestPlaceEntities db = new BestPlaceEntities();

        private const string logoffUrl = "http://localhost:4624";
        private const string redirectUrl = "http://localhost:4624/Account/OAuth";
        //private const string oauthURL = "https://www.facebook.com/dialog/oauth?client_id=364758183571073&redirect_uri=http://localhost:4624/Account/GetData&scope=email,offline_access&response_type=token";

        //private const string logoffUrl = "http://bestplace.vn/";
        //private const string redirectUrl = "http://bestplace.vn/Account/OAuth";
        //private const string oauthURL = "https://www.facebook.com/dialog/oauth?client_id=339362442795843&redirect_uri=http://lab.bestplace.vn/Account/GetData&scope=email,offline_access&response_type=token";

        //
        // GET: /Account/LogOn/
        public ActionResult LogOn(string returnUrl)
        {
            return RedirectToAction("BpLogOn", new { returnUrl = returnUrl });
        }


        public ActionResult FbLogOn(string returnUrl)
        {
            var oAuthClient = new FacebookOAuthClient(FacebookApplication.Current);
            oAuthClient.RedirectUri = new Uri(redirectUrl);
            var loginUri = oAuthClient.GetLoginUrl(new Dictionary<string, object> { { "state", returnUrl }, { "scope", "email,user_birthday,offline_access" } });

            return Redirect(loginUri.AbsoluteUri);
        }

        //
        // GET: /Account/OAuth/

        public ActionResult OAuth(string code, string state)
        {
            FacebookOAuthResult oauthResult;
            if (FacebookOAuthResult.TryParse(Request.Url, out oauthResult))
            {
                if (oauthResult.IsSuccess)
                {
                    var oAuthClient = new FacebookOAuthClient(FacebookApplication.Current);
                    oAuthClient.RedirectUri = new Uri(redirectUrl);

                    dynamic tokenResult = oAuthClient.ExchangeCodeForAccessToken(code);
                    string accessToken = tokenResult.access_token;

                    DateTime expiresOn = DateTime.MaxValue;

                    if (tokenResult.ContainsKey("expires"))
                    {
                        DateTimeConvertor.FromUnixTime(tokenResult.expires);
                    }

                    FacebookClient fbClient = new FacebookClient(accessToken);
                    dynamic me = fbClient.Get("me?fields=id,name,email,birthday,gender");
                    long facebookId = Convert.ToInt64(me.id);

                    InMemoryUserStore.Add(new FacebookUser
                    {
                        AccessToken = accessToken,
                        Expires = expiresOn,
                        FacebookId = facebookId,
                        Name = (string)me.name,
                    });

                    var user = Membership.GetUser(facebookId.ToString());

                    FormsAuthentication.SetAuthCookie(facebookId.ToString(), false);
                    
                    string format = "d";
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime birthday = new DateTime();
                    try
                    {
                        birthday = DateTime.ParseExact(me.birthday, format, provider);
                    }
                    catch
                    {
                    }

                    if (user == null)
                    {
                        var u = Membership.CreateUser(facebookId.ToString(), Guid.NewGuid().ToString());
                        using (BestPlaceEntities db = new BestPlaceEntities())
                        {
                            db.bp_Profile_Create((Guid)u.ProviderUserKey,
                                facebookId.ToString(),
                                (string)me.name,
                                Transfer.GetPictureUrl(facebookId.ToString()),
                                (string)me.email,
                                null,
                                birthday,
                                ((string)me.gender == "male") ? true : false,
                                null, null);
                        }

                    }
                    else
                    {
                        using (BestPlaceEntities db = new BestPlaceEntities())
                        {
                            db.bp_Profile_Update((Guid)user.ProviderUserKey,
                                (string)me.name,
                                (string)me.email,
                                null,
                                birthday,
                                ((string)me.gender == "male") ? true : false,
                                null, null);
                        }
                    }

                    // prevent open redirection attack by checking if the url is local.
                    if (Url.IsLocalUrl(state))
                    {
                        return Redirect(state);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/LogOff/

        public ActionResult LogOff()
        {
            var facebookId = long.Parse(User.Identity.Name);
            var user = InMemoryUserStore.Get(facebookId);

            FormsAuthentication.SignOut();
            if (user != null)
            {
                var logoutUrl = String.Format("https://www.facebook.com/logout.php?next={0}&access_token={1}", logoffUrl, user.AccessToken);
                return Redirect(logoutUrl);
            }
            else
            {
                return Redirect(logoffUrl);
            }

            
            //var oAuthClient = new FacebookOAuthClient(FacebookApplication.Current);
            //oAuthClient.RedirectUri = new Uri(logoffUrl);
            //var logoutUrl = oAuthClient.GetLogoutUrl();
            //return Redirect(logoutUrl);//.AbsoluteUri);
        }
        //Add

        public ActionResult BpLogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BpLogOn(LogOnModel logOn, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(logOn.UserName, logOn.Password))
                {
                    FormsAuthentication.SetAuthCookie(logOn.UserName, false);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(logOn);
        }

        public ActionResult BpLogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            RegisterModel register = new RegisterModel();
            return View(register);
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var user = Membership.CreateUser(model.Email, model.Password, model.Email);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);

                    db.bp_Profile_Create(
                        (Guid)user.ProviderUserKey,
                        "bp",
                        model.UserName,
                        "bp.png",
                        model.Email,
                        null,
                        null,
                        null,
                        null,
                        null);

                    return RedirectToAction("Index", "Home");
                }
                else
                {

                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}
