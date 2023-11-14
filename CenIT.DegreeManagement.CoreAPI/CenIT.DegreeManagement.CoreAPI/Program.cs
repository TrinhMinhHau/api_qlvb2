//using CenIT.DegreeManagement.CoreAPI.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Hubs;
using CenIT.DegreeManagement.CoreAPI.Middleware;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Notification;
using CenIT.DegreeManagement.CoreAPI.Processor;
using CenIT.DegreeManagement.CoreAPI.Processor.Mail;
using CenIT.DegreeManagement.CoreAPI.Processor.SendNotification;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        c.OperationFilter<AddHeaderOperationFilter>();
    }
);

// config Mail
builder.Services.AddOptions();                                       
var mailsettings = builder.Configuration.GetSection("MailSettings");  
builder.Services.Configure<MailSettings>(mailsettings);

builder.Services.AddMvc();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheLayer>();
builder.Services.AddSingleton<ShareResource>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<ISendMailService, SendMailService>();
//Firebase
var googleCredentialPath = builder.Environment.ContentRootPath;
var filePath = builder.Configuration.GetSection("GoogleFirebase")["FileName"];
var googleCredential = Path.Combine(googleCredentialPath, filePath);

var credential = GoogleCredential.FromFile(googleCredential);
FirebaseApp.Create(new AppOptions()
{
    Credential = credential
});

builder.Services.AddScoped<FirebaseNotificationUtils>();
builder.Services.AddTransient<BackgroundJobManager>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

var connection = builder.Configuration.GetConnectionString("qlvanbang");

builder.Services.AddHangfire(config =>
                config.UsePostgreSqlStorage(connection));


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Add services and config logger
var _logger = new LoggerConfiguration().ReadFrom
    .Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();
builder.Logging.AddSerilog(_logger);


// Configure strongly typed settings objects


//Add services localization 
builder.Services.AddLocalization();
var localizationOptions = new RequestLocalizationOptions();

var supportedCultures = new[]
{
    new CultureInfo("vi-VN"),
    new CultureInfo ("en-US")
};
localizationOptions.SupportedCultures = supportedCultures;
localizationOptions.SupportedUICultures = supportedCultures;
localizationOptions.SetDefaultCulture("vi-VN");
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;


builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

var app = builder.Build();


app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHangfireServer();
app.UseHangfireDashboard();


////app.Urls.Add("http://localhost:5024");

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseCors(builder => builder
        .AllowAnyOrigin() // Cho phép tất cả các origin
        .AllowAnyMethod()
        .AllowAnyHeader() // Cho phép tất cả các header
);


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Resources"
});


app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<EnableRequestBodyBufferingMiddleware>();

app.Run();
