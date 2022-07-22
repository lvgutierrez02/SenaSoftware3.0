using System.Text;
using Contracts;
using Entities;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;

namespace Seguimiento.API.Extensions
{
    public static class ServiceExtensions  //contenedor de inyección de dependencias de la aplicación
    {

        //CORS: Cross-Origin Resource Sharing =  //es un mecanismo para dar o restringir derechos de acceso a aplicaciones de diferentes dominios.
        public static void ConfigureCors(this IServiceCollection services) =>
         services.AddCors(options =>
         {
             options.AddPolicy("CorsPolicy", builder =>
             builder.AllowAnyOrigin()//permite peticiones de cualquier origen
             .AllowAnyMethod()//permite todos los metodos HTTP
             .AllowAnyHeader());//permite todas las cabeceras
         });

        //configura una integración con IIS que nos ayudará finalmente con el despliegue en IIS
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        //agregando la cadena de conexion con la db Sql
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));


        
        //Se agrega el administrador de Repository
        public static void ConfigureRepositoryManager(this IServiceCollection services) => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            //crear un servicio la primera vez que lo solicitamos y luego cada solicitud posterior llamará a la misma instancia del servicio.
        }


        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            }
            );
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole),
                builder.Services);
            builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        }


        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // Primero, extraemos JwtSettings del archivo appsettings.json y extraemos nuestra variable de entorno
            // (si sigue obteniendo un valor nulo para la clave secreta, intente reiniciar Visual Studio o incluso su computadora).
            var jwtSettings = configuration.GetSection("JwtSettings");
            //var secretKey = Environment.GetEnvironmentVariable("SECRET");
            byte[] key = Encoding.UTF8.GetBytes(jwtSettings.GetSection("keyjwt").ToString());
            services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false, // La audiencia de un token es el destinatario previsto del token. Debemos cambiar a true cuando se trabaje en producción
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    //ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }
    }
}
