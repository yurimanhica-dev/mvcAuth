namespace Autentication.Helpers;

public class LockScreenMiddleware
{
    private readonly RequestDelegate _next;

    public LockScreenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var isLocked = context.Session.GetString("LOCKED") == "1";
        var path = context.Request.Path.Value?.ToLower();

        // Permite acesso apenas a UnlockScreen ou LockScreen
        if (isLocked && path != "/account/unlockscreen" && path != "/account/lockscreen")
        {
            context.Response.Redirect("/Account/LockScreen");
            return;
        }

        await _next(context);
    }
}
