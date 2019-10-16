using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SqlDemo.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using SqlDemo.Services;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SqlDemo.ViewModels;

namespace SqlDemo.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser<Guid>> userManager;
        private readonly SignInManager<IdentityUser<Guid>> signInManager;
        private readonly IMessageService messageService;

        public AccountController(UserManager<IdentityUser<Guid>> userManager, SignInManager<IdentityUser<Guid>> signInManager, IMessageService messageService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.messageService = messageService;
        }

        public async Task<IActionResult> Index()
        {
            AccountViewModel model = new AccountViewModel();
            model.User = await this.userManager.GetUserAsync(HttpContext.User);
            model.Claims = await this.userManager.GetClaimsAsync(model.User);
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string repassword)
        {
            if (password != repassword)
            {
                ModelState.AddModelError(string.Empty, "Password don't match");
                return View();
            }

            var newUser = new IdentityUser<Guid> 
            {
                UserName = email,
                Email = email
            };

            var userCreationResult = await this.userManager.CreateAsync(newUser, password);
            if (!userCreationResult.Succeeded)
            {
                foreach(var error in userCreationResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();
            }

            var emailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(newUser);
            //await this.userManager.AddClaimAsync(identityUser, new Claim(ClaimTypes.Role, "Administrator"));
            var tokenVerificationUrl = Url.Action("VerifyEmail", "Account", new {id = newUser.Id, token = emailConfirmationToken}, Request.Scheme);

            await this.messageService.Send(email, "Verify your email", $"Click <a href=\"{tokenVerificationUrl}\">here</a> to verify your email");

            return Content("Check your email for a verification link");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddRole(string role)
        {
            Console.WriteLine("\r\nAccountController::AddRole: role={0}\r\n", role);
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new InvalidOperationException();
            }
            Console.WriteLine("\r\nAccountController::AddRole: role={0}, user={1}\r\n", role, user);
            var claims = await this.userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Value.Equals(role)))
            {
                return Content("user already has role");
            }
            await this.userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string role)
        {
            Console.WriteLine("\r\nAccountController::RemoveRole: role={0}\r\n", role);
            var user = await this.userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new InvalidOperationException();
            }
            Console.WriteLine("\r\nAccountController::RemoveRole: role={0}, user={1}\r\n", role, user);
            await this.userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, role));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            var user = await this.userManager.FindByIdAsync(id);
            if(user == null)
                throw new InvalidOperationException();

            var emailConfirmationResult = await this.userManager.ConfirmEmailAsync(user, token);
            if (!emailConfirmationResult.Succeeded)            
                return Content(emailConfirmationResult.Errors.Select(error => error.Description).Aggregate((allErrors, error) => allErrors += ", " + error));                            

            return Content("Email confirmed, you can now log in");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                return View();
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email first");
                return View();
            }

            var passwordSignInResult = await this.signInManager.PasswordSignInAsync(user, password, isPersistent: rememberMe, lockoutOnFailure: false);
            if (!passwordSignInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                return View();                
            }

            return Redirect("~/");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await this.userManager.FindByEmailAsync(email);
            if (user == null)
                return Content("Check your email for a password reset link");

            var passwordResetToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetUrl = Url.Action("ResetPassword", "Account", new {id = user.Id, token = passwordResetToken}, Request.Scheme);

            await this.messageService.Send(email, "Password reset", $"Click <a href=\"" + passwordResetUrl + "\">here</a> to reset your password");

            return Content("Check your email for a password reset link");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id, string token, string password, string repassword)
        {           
            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
                throw new InvalidOperationException();

            if (password != repassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return View();
            }

            var resetPasswordResult = await this.userManager.ResetPasswordAsync(user, token, password);
            if (!resetPasswordResult.Succeeded)
            {
                foreach(var error in resetPasswordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View();                
            }

            return Content("Password updated");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return Redirect("~/");
        }    
    }
}