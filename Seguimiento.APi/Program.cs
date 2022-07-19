using AutoMapper;
using Microsoft.AspNetCore.HttpOverrides;
using NLog; //NLog es una plataforma de registro para .NET que nos ayudará a crear y registrar nuestros mensajes
using Seguimiento.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config")); // logger para cargar el archivo

// Add services to the container.

builder.Services.ConfigureCors();

builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureLoggerService(); //se agrega el servicio de registro

var configuration = builder.Configuration;
builder.Services.ConfigureSqlContext(configuration); //Agregando el metodo que contiene la conexion con la DB 

builder.Services.ConfigureRepositoryManager(); //Se agrega el admin repository como servicio




//builder.Services.AddControllers(config => //Anula la configuracion donde formatea una respuesta a JSON
//{
//    config.RespectBrowserAcceptHeader = true; //decirle al servidor que respete la cabecera Accept

//    //Indica al servidor que si el cliente intenta negociar para el tipo de medio que el tipo de medio que el servidor no soporta, debe devolver el codigo
//    //de estado 406 Not Acceptable No aceptable.Esto hara que nuestra aplicaci�n sea m�s restrictiva y obligario al consumidor de la API a solicitar solo los tipos
//    //que el servidor admite. El codigo de estado 406 se ha creado con este prop�sito.
//    //config.ReturnHttpNotAcceptable = true;  
//    //builder.Services.AddControllers(config => //Anula la configuracion donde formatea una respuesta a JSON
//    //{
//    //    config.RespectBrowserAcceptHeader = true; //decirle al servidor que respete la cabecera Accept

//}).AddXmlDataContractSerializerFormatters(); //para soportar los formateadores XML



builder.Services.AddControllers();

var app = builder.Build();



// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();
