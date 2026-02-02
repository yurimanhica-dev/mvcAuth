using System.Security.Claims;

using Autentication.Models;
using Autentication.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Autentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly IEmailService emailService;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailService = emailService;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Overview", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password incorrect");
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users { FullName = model.Name, Email = model.Email, UserName = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                //verifica o email na base de dados e envia o email com o token para alterar a password
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email not found!");
                    return View(model);
                }

                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ChangePasswordByEmail", "Account", new { username = model.Email, token = resetToken }, Request.Scheme);

                var subject = "Password Reset";
                var body = $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>";
                //envia o email com o link para alterar a password
                await emailService.SendEmailAsync(model.Email, subject, body);

                return RedirectToAction("EmailSent", "Account");

                //verifica o email direto na base de dados e retorna o form de alterar password
                // var user = await userManager.FindByEmailAsync(model.Email);

                // if (user == null)
                // {
                //     ModelState.AddModelError(string.Empty, "User not found");
                //     return View(model);
                // }
                // else
                // {
                //     return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                // }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePasswordByEmail(string username, string token)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            var model = new ChangePasswordViewModel
            {
                Email = username,
                Token = token
            };
            return View(model);
        }
        //valido quando verifica o email direto na base de dados e retorna o form de alterar password
        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordByEmail(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong, try again!");
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email not found!");
                return View(model);
            }
            var resetResult = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        //valido quando verifica o email direto na base de dados e retorna o form de alterar password
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email not found!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Something went wrong, try again!");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult EmailSent()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}