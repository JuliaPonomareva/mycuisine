using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCuisine.Web.Constants;
using MyCuisine.Web.Data;
using MyCuisine.Web.Extensions;
using MyCuisine.Web.Helpers;
using MyCuisine.Web.Models;
using System.Security.Claims;

namespace MyCuisine.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly AppSettings _appSettings;

        public AccountController(ApplicationContext dbContext, ILogger<AccountController> logger,
            AppSettings appSettings)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings;
        }

        [HttpGet]
        public IActionResult Login([FromQuery] string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model?.IdToken == null)
            {
                return BadRequest();
            }

            var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_appSettings.FirebaseAdminConfig);
            FirebaseToken decoded = await firebaseAuth.VerifyIdTokenAsync(model.IdToken);
            var email = decoded.Claims["email"].ToString().ToLower();
            var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Email == email);
            if (user == null)
            {
                var name = decoded.Claims.TryGetValue("name", out object val)
                    ? (string)val
                    : email.Split('@').First();

                user = new MyCuisine.Data.Web.Models.User
                {
                    Email = email,
                    Name = string.Join("", name.Take(50)),
                    IsActive = true,
                    IsAdmin = false,
                    DateCreated = DateTimeOffset.Now,
                    DateModified = DateTimeOffset.Now
                };
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            else if (!user.IsActive)
            {
                HttpContext.SetError("Пользователь заблокирован.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(AuthConstants.FirebaseIdClaim, decoded.Uid),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            };

            if (user.IsAdmin)
            {
                claims.Add(new Claim(AuthConstants.AdminClaim, "true"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/" : model.ReturnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
            var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(new ViewModel<UserViewModel>
            {
                Data = new UserViewModel
                {
                    Email = user.Email,
                    UpdateNameForm = new UserViewModel.UpdateNameFormModel
                    {
                        Name = user.Name
                    }
                }
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(ViewModel<UserViewModel> model)
        {
            if (model?.Data?.UpdateNameForm == null && model?.Data?.UpdatePasswordForm == null)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
            var user = await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(model.Data.UpdateNameForm?.Name))
                {
                    user.Name = model.Data.UpdateNameForm.Name;
                    user.DateModified = DateTimeOffset.Now;
                    await _dbContext.SaveChangesAsync();
                }

                if (!string.IsNullOrWhiteSpace(model.Data.UpdatePasswordForm?.Password))
                {
                    var firebaseAuth = FirebaseHelper.GetFirebaseAuth(_appSettings.FirebaseAdminConfig);
                    var userFirebaseId = User.Claims.First(s => s.Type == AuthConstants.FirebaseIdClaim).Value;
                    await firebaseAuth.UpdateUserAsync(new UserRecordArgs
                    {
                        Uid = userFirebaseId,
                        Password = model.Data.UpdatePasswordForm.Password
                    });
                }

                return RedirectToAction(nameof(Settings));
            }

            return View(new ViewModel<UserViewModel>
            {
                Data = new UserViewModel
                {
                    Email = user.Email,
                    UpdateNameForm = new UserViewModel.UpdateNameFormModel
                    {
                        Name = user.Name
                    }
                }
            });
        }
    }
}