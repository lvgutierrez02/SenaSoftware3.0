using System.Net;
using Contracts;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Diagnostics;

namespace Seguimiento.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            //Creamos un método de extensión en el que registramos el middleware UseExceptionHandler, es un middleware integrado
            //que podemos usar para manejar excepciones
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    //completamos el código de estado y el tipo de contenido de nuestra respuesta,
                    //registramos el mensaje de error y finalmente devolvimos la respuesta con el objeto personalizado creado.
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());
                    }
                });
            });
        }
    }
}
