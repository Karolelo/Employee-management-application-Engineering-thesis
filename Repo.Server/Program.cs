using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Repo.Server.Controllers;
using Repo.Server.Controllers.Interfaces;
using Repo.Server.TaskModule;
using Repo.Server.TaskModule.interafaces;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//adding services
builder.Services.AddScoped<IAuthUserService,AuthUserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<AuthenticationHelpers>();

// Connection priority - changeable if needed
var candidateNames = new[] { "Mroziu-workspace", "DefaultConnection" };

// Collect the connection strings from config
var candidates = candidateNames
    .Select(n => (Name: n, Conn: builder.Configuration.GetConnectionString(n)))
    .Where(x => !string.IsNullOrWhiteSpace(x.Conn))
    .ToList();

if (candidates.Count == 0)
{
    throw new InvalidOperationException("No defined ConnectionStrings");
}

// Choosing the first working connection string
var chosen = await ChooseFirstWorkingAsync(candidates);
Console.WriteLine($"[DB] Chosen ConnectionString: {chosen.Name}");

// Ensure the ApplicationDbContext is registered as a service
builder.Services.AddDbContext<MyDbContext>(conf =>
    conf.UseSqlServer(chosen.Conn, o => o.EnableRetryOnFailure()));
// builder.Services.AddDbContext<MyDbContext>(conf=> conf
//     .UseSqlServer(builder
//         .Configuration
//         .GetConnectionString("DefaultConnection"))); ;
// Adding Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

// Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
        };
    });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Write JWT Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevServer",
        builder =>
        {
            builder.WithOrigins("http://localhost:55399")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty; 
    });
}

static async Task<(string Name, string Conn)> ChooseFirstWorkingAsync(
    IEnumerable<(string Name, string Conn)> candidates)
{
    foreach (var c in candidates)
    {
        try
        {
            // Shorten the ConnectTimeout for testing the connection only
            var sb = new SqlConnectionStringBuilder(c.Conn) { ConnectTimeout = 2 };

            await using var conn = new SqlConnection(sb.ConnectionString);
            await conn.OpenAsync(); // If succeeded, take that ConnectionString
            await conn.CloseAsync();

            return (c.Name, sb.ConnectionString);
        }
        catch (Exception e)
        {
            // Ignore and try out another one
        }
    }

    throw new InvalidOperationException("Failed to find a working ConnectionString");
}
app.UseCors("AllowAngularDevServer");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
