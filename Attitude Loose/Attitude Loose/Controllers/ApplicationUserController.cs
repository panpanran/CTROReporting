using Attitude_Loose.Models;
using Attitude_Loose.Properties;
using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class ApplicationUserController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IUserProfileService userProfileService;
        private IUserService userService;

        public ApplicationUserController(IUserProfileService userProfileService, IUserService userService, UserManager<ApplicationUser> userManager)
        {
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            this.userManager = userManager;
            this.userProfileService = userProfileService;
            this.userService = userService;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //if (Request.QueryString["guid"] != null)
            //    SocialGoalSessionFacade.JoinGroupOrGoal = Request.QueryString["guid"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: ApplicationUser
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var userId = user.Id;
                    var Email = user.Email;
                    userProfileService.CreateUserProfile(userId, Email);
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private IAuthenticationManager _authnManager;

        // Modified this from private to public and add the setter
        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                if (_authnManager == null)
                    _authnManager = HttpContext.GetOwinContext().Authentication;
                return _authnManager;
            }
            set { _authnManager = value; }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            System.Web.HttpContext.Current.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [OutputCache(Duration = 5)]
        public ViewResult UserProfile(string id)
        {
            var currentuserid = User.Identity.GetUserId();
            var user = userService.GetByUserID(id);
            var userProfile = userProfileService.GetByUserID(id);
            UserProfileViewModel userprofile = new UserProfileViewModel()
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Email = userProfile.Email,
                UserName = user.UserName,
                DateCreated = user.DateCreated,
                LastLoginTime = user.LastLoginTime,
                UserId = user.Id,
                ProfilePicUrl = user.ProfilePicUrl,
                DateOfBirth = userProfile.DateOfBirth,
                Gender = userProfile.Gender,
                Address = userProfile.Address,
                City = userProfile.City,
                State = userProfile.State,
                Country = userProfile.Country,
                ZipCode = userProfile.ZipCode,
                ContactNo = userProfile.ContactNo,
            };
            return View(userprofile);
        }

        public ActionResult EditProfile()
        {
            var user = userProfileService.GetByUserID(User.Identity.GetUserId());
            UserProfileFormModel editUser = Mapper.Map<UserProfile, UserProfileFormModel>(user);
            editUser.LocalPath = userService.GetByUserID(User.Identity.GetUserId()).ProfilePicUrl;

            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("EditProfile", editUser);
        }

        [HttpPost]
        public ActionResult UpdateProfile(UserProfileFormModel updateProfile)
        {
            UserProfile user = Mapper.Map<UserProfileFormModel, UserProfile>(updateProfile);
            ApplicationUser applicationUser = userService.GetByUserID(updateProfile.UserId);
            applicationUser.FirstName = updateProfile.FirstName;
            applicationUser.LastName = updateProfile.LastName;
            applicationUser.Email = updateProfile.Email;
            UploadImage(updateProfile);
            userService.UpdateUser(applicationUser);
            userProfileService.UpdateUserProfile(user);
            return RedirectToAction("UserProfile", new { id = updateProfile.UserId });
        }

        [HttpPost]
        public void UploadImage(UserProfileFormModel model)
        {
            //Prepare the needed variables
            Bitmap original = null;
            var name = "newimagefile";
            var errorField = string.Empty;

            if (model.IsUrl)
            {
                errorField = "Url";
                name = GetUrlFileName(model.Url);
                original = GetImageFromUrl(model.Url);
            }
            else if (model.File != null)
            {
                errorField = "File";
                name = Path.GetFileNameWithoutExtension(model.File.FileName);
                original = Bitmap.FromStream(model.File.InputStream) as Bitmap;
            }

            //If we had success so far
            if (original != null)
            {
                var img = CreateImage(original, model.X, model.Y, model.Width, model.Height);
                var fileName = Guid.NewGuid().ToString();
                var oldFilepath = userService.GetByUserID(User.Identity.GetUserId()).ProfilePicUrl;
                var oldFile = Server.MapPath(oldFilepath);
                //Demo purposes only - save image in the file system
                var fn = Server.MapPath("~/Images/ProfilePics/" + fileName + ".png");
                img.Save(fn, System.Drawing.Imaging.ImageFormat.Png);
                userService.SaveImageURL(User.Identity.GetUserId(), "~/Images/ProfilePics/" + fileName + ".png");
                if (System.IO.File.Exists(oldFile))
                {
                    System.IO.File.Delete(oldFile);
                }
            }
            else //Otherwise we add an error and return to the (previous) view with the model data
                ModelState.AddModelError(errorField, Resources.UploadError);
            //return RedirectToAction("UserProfile", new { id = User.Identity.GetUserId() });
        }

        Bitmap CreateImage(Bitmap original, int x, int y, int width, int height)
        {
            var img = new Bitmap(width, height);

            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(original, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);
            }

            return img;
        }

        /// <summary>
        /// Gets the filename that is placed under a certain URL.
        /// </summary>
        /// <param name="url">The URL which should be investigated for a file name.</param>
        /// <returns>The file name.</returns>
        string GetUrlFileName(string url)
        {
            var parts = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var last = parts[parts.Length - 1];
            return Path.GetFileNameWithoutExtension(last);
        }

        /// <summary>
        /// Gets an image from the specified URL.
        /// </summary>
        /// <param name="url">The URL containing an image.</param>
        /// <returns>The image as a bitmap.</returns>
        Bitmap GetImageFromUrl(string url)
        {
            var buffer = 1024;
            Bitmap image = null;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return image;

            using (var ms = new MemoryStream())
            {
                var req = WebRequest.Create(url);

                using (var resp = req.GetResponse())
                {
                    using (var stream = resp.GetResponseStream())
                    {
                        var bytes = new byte[buffer];
                        var n = 0;

                        while ((n = stream.Read(bytes, 0, buffer)) != 0)
                            ms.Write(bytes, 0, n);
                    }
                }

                image = Bitmap.FromStream(ms) as Bitmap;
            }

            return image;
        }
    }
}