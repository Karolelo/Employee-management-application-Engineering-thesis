using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Repo.Core.Infrastructure.Files;
using Repo.Server.CalendarModule.Interfaces;
using Repo.Server.CalendarModule.Repositories;
using Repo.Server.CalendarModule.Services;
using Repo.Server.Controllers;
using Repo.Server.Controllers.Interfaces;
using Repo.Server.TaskModule;
using Repo.Server.TaskModule.interafaces;
using Repo.Server.UserManagmentModule.Interfaces;
using Repo.Server.UserManagmentModule.Repository;
using Repo.Server.UserManagmentModule.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

//adding services
builder.Services.AddScoped<IAuthUserService,AuthUserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<AuthenticationHelpers>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICalendarService,CalendarService>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IRoleRepository,RoleRepository>();
builder.Services.AddScoped<IGroupRepository,GroupRepository>();
builder.Services.AddScoped<IGroupService,GroupService>();
builder.Services.AddScoped<IFileOperations,FileOperation>();

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
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
//adding basic roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User","TeamLeader","Admin","Accountant"));
    options.AddPolicy("TeamLeaderOnly",policy => policy.RequireRole("TeamLeader","Admin"));
    options.AddPolicy("AccountantOnly", policy => policy.RequireRole("Admin", "Accountant"));
    
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
    app.UseHttpsRedirection();
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
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/images"))
    {
        Console.WriteLine($"Próba dostępu do ścieżki: {context.Request.Path}");
    }
    await next();
});
app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseRouting();

app.UseCors("AllowAngularDevServer");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
