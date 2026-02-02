using Autentication.ViewModels;

using Microsoft.AspNetCore.Mvc;

public class SignupFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new RegisterViewModel());
    }
}
