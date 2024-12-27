using JarvisWeb.Components;
using JarvisWeb.Domain;
using JarvisWeb.Services.Adapters.Calendar;
using JarvisWeb.Services.Adapters.LLM;
using JarvisWeb.Services.Adapters.News;
using JarvisWeb.Services.Adapters.Transcription;
using JarvisWeb.Services.Adapters.Weather;
using JarvisWeb.Services.Interfaces;
using JarvisWeb.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddDbContext<JarvisWebDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JarvisWebDb")));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();
builder.Services.AddScoped<INewsService, TheNewsApiService>();
builder.Services.AddScoped<ICalendarService, HomeAssistantCalendarService>();
builder.Services.AddScoped<IWeatherService, NOAAWeatherService>();
builder.Services.AddScoped<ILLMService, OLLamaService>();
builder.Services.AddScoped<ITranscriptionService, WhisperService>();
builder.Services.AddScoped<DailySummaryService>();
builder.Services.AddScoped<EndOfDayNoteService>();
builder.Services.AddScoped<ApiKeyService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<GlobalStateService>();
builder.Services.AddHttpContextAccessor();

// add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "JarvisWeb", Version = "v1" });
    //// Bearer token authentication
    //var securityDefinition = new OpenApiSecurityScheme()
    //{
    //    Name = "Bearer",
    //    BearerFormat = "JWT",
    //    Scheme = "bearer",
    //    Description = "Specify the authorization token.",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.Http,
    //};
    //c.AddSecurityDefinition("jwt_auth", securityDefinition);

    //// Make sure swagger UI requires a Bearer token specified
    //var securityScheme = new OpenApiSecurityScheme()
    //{
    //    Reference = new OpenApiReference()
    //    {
    //        Id = "jwt_auth",
    //        Type = ReferenceType.SecurityScheme,
    //    },
    //};
    //var securityRequirements = new OpenApiSecurityRequirement()
    //{
    //    { securityScheme, new List<string>() },
    //};
    //c.AddSecurityRequirement(securityRequirements);

    // Set the comments path for the Swagger JSON and UI.
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Audience = builder.Configuration["Jwt:Firebase:ValidAudience"];
        opt.Authority = builder.Configuration["Jwt:Firebase:ValidIssuer"];
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Firebase:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:Firebase:ValidAudience"]
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JarvisWeb v1");
});

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<JarvisWebDbContext>();
    dataContext.Database.Migrate();
}

app.UseRouting();
app.UseAntiforgery();

app.Run();
