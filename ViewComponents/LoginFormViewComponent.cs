using Autentication.ViewModels;

using Microsoft.AspNetCore.Mvc;

public class LoginFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new LoginViewModel());
    }
}
