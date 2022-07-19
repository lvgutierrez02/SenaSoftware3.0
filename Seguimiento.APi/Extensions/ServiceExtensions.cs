using Contracts;
using Entities;
using LoggerService;

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

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.Add<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));



        //Se agrega el administrador de Repository
        public static void ConfigureRepositoryManager(this IServiceCollection services) => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            //crear un servicio la primera vez que lo solicitamos y luego cada solicitud posterior llamará a la misma instancia del servicio.
        }
    }
}
