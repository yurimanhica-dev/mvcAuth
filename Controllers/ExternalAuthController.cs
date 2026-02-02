using System.Security.Claims;

using Autentication.Helpers;
using Autentication.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class ExternalAuthController : Controller
{
    private readonly SignInManager<Users> _signInManager;
    private readonly UserManager<Users> _userManager;

    public ExternalAuthController(
        SignInManager<Users> signInManager,
        UserManager<Users> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "ExternalAuth");
        var properties = _signInManager
            .ConfigureExternalAuthenticationProperties("Google", redirectUrl);

        return Challenge(properties, "Google");
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("Login", "Account");

        // Tenta login externo
        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: false
        );

        if (signInResult.Succeeded)
        {
            // Usuário já existe, login ok
            return RedirectToAction("Overview", "Dashboard");
        }

        // Usuário não existe, cria
        var email = info.Principal.FindFirstValue(ClaimTypes.Email)!;
        var profilePicture = info.Principal.FindFirstValue("urn:google:picture");
        var fullNameNormal = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "User";
        var fullName = StringUtils.ToTitleCase(fullNameNormal);

        // Checa se o usuário já existe no banco (por email)
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            // Se já existir, apenas adiciona login externo
            await _userManager.UpdateAsync(existingUser);
            await _userManager.AddLoginAsync(existingUser, info);
            await _signInManager.SignInAsync(existingUser, false);
            return RedirectToAction("Overview", "Dashboard");
        }

        // Cria novo usuário
        var user = new Users
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            ProfilePicture = profilePicture ?? "https://dbmib2q8rj.ufs.sh/f/Lm6xK3J7O1CLN3IBelWpIWkVAnQLmybZp4cdleuBgOMf7C3J"
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            // Loga erros ou redireciona para login
            foreach (var error in createResult.Errors)
            {
                Console.WriteLine(error.Description);
            }
            return RedirectToAction("Login", "Account");
        }

        // Adiciona login externo
        await _userManager.AddLoginAsync(user, info);

        // Faz login do usuário
        await _signInManager.SignInAsync(user, false);

        return RedirectToAction("Overview", "Dashboard");
    }
}
