namespace SocietyLedger.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseCustomMiddleware(this WebApplication app)
        {
            // Example of chaining middlewares
            app.UseMiddleware<SocietyLedger.Api.Middlewares.ExceptionMiddleware>();
            return app;
        }
    }
}
