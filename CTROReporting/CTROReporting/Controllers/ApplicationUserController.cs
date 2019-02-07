using CTROLibrary.DBbase;
using CTROLibrary.Model;
using CTROReporting.Service;
using CTROReporting.ViewModels;
using AutoMapper;
using CTROReporting.Properties;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CTROLibrary.Infrastructure;
using System.Configuration;

namespace CTROReporting.Controllers
{
    [Authorize]
    public class ApplicationUserController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private IUserProfileService userProfileService;
        private IUserService userService;
        private IDepartmentService departmentService;
        private IScheduleService scheduleService;

        public ApplicationUserController(IScheduleService scheduleService, IUserProfileService userProfileService, IUserService userService, IDepartmentService departmentService, UserManager<ApplicationUser> userManager)
        {
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            this.userManager = userManager;
            this.departmentService = departmentService;
            this.userProfileService = userProfileService;
            this.userService = userService;
            this.scheduleService = scheduleService;
        }

        public ActionResult Test()
        {
            return View();
        }

        [AllDepartmentAuthorize]
        public ActionResult UserManagement(UserManagementViewModel model)
        {
            var users = userService.GetUsers();
            var usersList = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserManagementViewModel>>(users).ToList();
            return View(usersList);
        }

        //public ActionResult ActivatedSelection()
        //{
        //    var activated = new List<ApplicationUser> { new ApplicationUser { Activated = true }, new ApplicationUser { Activated = false } };
        //    return Json(activated, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult UpdateUser(UserManagementViewModel model)
        {
            ApplicationUser user = userService.GetByUserID(model.Id);
            user.Activated = Convert.ToBoolean(model.Activated);
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.DepartmentId = departmentService.GetByDepartmentName(model.DepartmentName).DepartmentId;
            userService.UpdateUser(user);
            UserProfile userprofile = userProfileService.GetByUserID(model.Id);
            userprofile.Email = model.Email;
            userProfileService.UpdateUserProfile(userprofile);
            return View();
        }

        public ActionResult DeleteUser(UserManagementViewModel model)
        {
            userService.DeleteUser(model.Id);
            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //CTROHangfire.Start();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

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
                    model.Activated = user.Activated;
                    if (user.Activated == true)
                    {
                        Response.Cookies["UserSettings"]["Department"] = user.Department.DepartmentName;
                        Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(1d);
                        await SignInAsync(user, model.RememberMe);
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "This user is inactive now. Please contact Ran Pan.");
                    }
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
                return RedirectToAction("Report", "Report");
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
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    LastLoginTime = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    DepartmentId = 4
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var userId = user.Id;
                    var Email = user.Email;
                    userProfileService.CreateUserProfile(userId, Email);

                    if (user.Activated == true)
                    {
                        await SignInAsync(user, isPersistent: false);

                        Response.Cookies["UserSettings"]["Department"] = departmentService.GetByDepartmentID(user.DepartmentId).DepartmentName;
                        Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(1d);
                        return RedirectToAction("Report", "Report");
                    }
                    else
                    {
                        ModelState.AddModelError("", "This user is created but still inactive now. Please contact Ran Pan.");
                    }
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
            return RedirectToAction("Report", "Report");
        }

        [HttpGet]
        [OutputCache(Duration = 5)]
        public ViewResult UserProfile(string id)
        {
            var user = userService.GetByUserID(id);
            var userProfile = userProfileService.GetByUserID(id);
            UserProfileViewModel userprofile = new UserProfileViewModel()
            {
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                Email = userProfile.Email,
                UserName = user.UserName,
                CreatedDate = user.CreatedDate,
                LastLoginTime = user.LastLoginTime,
                UserId = user.Id,
                ProfilePicUrl = userProfile.ProfilePicUrl,
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
            editUser.ProfilePicUrl = userProfileService.GetByUserID(User.Identity.GetUserId()).ProfilePicUrl;

            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("EditProfile", editUser);
        }

        [HttpPost]
        public ActionResult UpdateProfile(UserProfileFormModel updateProfile)
        {
            //UserProfile userprofile = Mapper.Map<UserProfileFormModel, UserProfile>(updateProfile);

            UserProfile userprofile = userProfileService.GetByUserID(updateProfile.UserId);
            userprofile.FirstName = updateProfile.FirstName;
            userprofile.LastName = updateProfile.LastName;
            userprofile.Email = updateProfile.Email;
            userprofile.Address = updateProfile.Address;
            userprofile.Email = updateProfile.Email;
            userprofile.City = updateProfile.City;
            userprofile.DateOfBirth = updateProfile.DateOfBirth;
            userprofile.Gender = updateProfile.Gender;
            userprofile.Address = updateProfile.Address;
            userprofile.State = updateProfile.State;
            userprofile.Country = updateProfile.Country;
            userprofile.ZipCode = updateProfile.ZipCode;
            userprofile.ContactNo = updateProfile.ContactNo;

            userprofile.ProfilePicUrl = SaveImage(updateProfile);
            userProfileService.UpdateUserProfile(userprofile);

            ApplicationUser applicationUser = userService.GetByUserID(User.Identity.GetUserId());
            applicationUser.Email = updateProfile.Email;
            userService.UpdateUser(applicationUser);
            return RedirectToAction("UserProfile", new { id = updateProfile.UserId });
        }

        public string SaveImage(UserProfileFormModel model)
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
                var folderName = Server.MapPath(ConfigurationManager.AppSettings["V_CTROProfileImage"] + "/ProfilePics/");
                var oldFilepath = folderName + userProfileService.GetByUserID(User.Identity.GetUserId()).ProfilePicUrl;

                //Demo purposes only - save image in the file system
                var filepath = Guid.NewGuid().ToString() + ".png";
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                img.Save(folderName + filepath, System.Drawing.Imaging.ImageFormat.Png);
                if (System.IO.File.Exists(oldFilepath))
                {
                    System.IO.File.Delete(oldFilepath);
                }
                return filepath;
            }
            else //Otherwise we add an error and return to the (previous) view with the model data
            {
                ModelState.AddModelError(errorField, Resources.UploadError);
                return model.ProfilePicUrl;
            }
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