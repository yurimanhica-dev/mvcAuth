using Microsoft.AspNetCore.Mvc;
using Autentication.ViewModels;

public class SignupFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(new RegisterViewModel());
    }
}
