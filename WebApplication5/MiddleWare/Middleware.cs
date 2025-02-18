//using Microsoft.AspNetCore.Http;
//using System.Threading.Tasks;

//public class AuthMiddleware
//{
//    private readonly RequestDelegate _next;

//    public AuthMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task Invoke(HttpContext context)
//    {
//        var accessToken = context.Request.Cookies["accessToken"];
//        var path = context.Request.Path.Value?.ToLower();

//        if (path == "/login" && !string.IsNullOrEmpty(accessToken))
//        {
//            context.Response.Redirect("/dashboard");
//            return;
//        }

//        if (path == "/dashboard" && string.IsNullOrEmpty(accessToken))
//        {
//            context.Response.Redirect("/login");
//            return;
//        }

//        await _next(context);
//    }
//}
