using ClaimsModule.API.Middleware;
using ClaimsModule.API.Services;
using ClaimsModule.Application;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Infrastructure;
using ClaimsModule.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Claims Module API",
        Version = "v1",
        Description = "Enterprise claims vertical-slice API."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }] = []
    });
});

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:4200"])
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaimsModuleDevSecretKey_ChangeInProduction_32chars!!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ClaimsModule",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ClaimsModule",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
    app.UseHangfireDashboard("/hangfire");
app.MapControllers();
app.MapHealthChecks("/health");
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
    await db.Database.MigrateAsync();
}

ClaimsModule.Infrastructure.DependencyInjection.RegisterRecurringJobs();
app.Run();
