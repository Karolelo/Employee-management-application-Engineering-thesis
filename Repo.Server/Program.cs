using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Repo.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Repo.Core.Infrastructure.Database;
using Repo.Core.Infrastructure.Files;
using Repo.Core.Infrastructure.Roles;
using Repo.Core.Infrastructure.UnityOfWork;
using Repo.Server.AuthModule.Interfaces;
using Repo.Server.CalendarModule.Interfaces;
using Repo.Server.CalendarModule.Repositories;
using Repo.Server.CalendarModule.Services;
using Repo.Server.Controllers.Interfaces;
using Repo.Server.GradeModule.Interfaces;
using Repo.Server.GradeModule.Services;
using Repo.Server.ProfileModule;
using Repo.Server.ProfileModule.Services;
using Repo.Server.TaskModule;
using Repo.Server.TaskModule.interafaces;
using Repo.Server.TaskModule.Repository;
using Repo.Server.UnityOfWork;
using Repo.Server.WorkTimeModule.Interfaces;
using Repo.Server.WorkTimeModule.Services;
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
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<ITargetService, TargetService>();
builder.Services.AddScoped<IUserGradeService, UserGradeService>();
builder.Services.AddScoped<IWorkEntryService, WorkEntryService>();
builder.Services.AddScoped<AuthenticationHelpers>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ICalendarService,CalendarService>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IRoleRepository,RoleRepository>();
builder.Services.AddScoped<IGroupRepository,GroupRepository>();
builder.Services.AddScoped<IGroupService,GroupService>();
builder.Services.AddScoped<IFileOperations,FileOperation>();
builder.Services.AddScoped<IAnnoucementService,AnnouncementService>();
builder.Services.AddScoped<IAnnoucementRepository,AnnoucementRepository>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUnityOfWork<MyDbContext>, UnityOfWork>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IPriorityService, PriorityService>();
builder.Services.AddScoped<IPriorityRepository, PriorityRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
//Creating getting a role from appseting
builder.Services.Configure<RoleConfiguration>(
    builder.Configuration.GetSection("Roles"));


// Connection priority - changeable if needed
//var candidateNames = new[] { "Mroziu-workspace", "DefaultConnection" };

// Collect the connection strings from config
/*var candidates = candidateNames
    .Select(n => (Name: n, Conn: builder.Configuration.GetConnectionString(n)))
    .Where(x => !string.IsNullOrWhiteSpace(x.Conn))
    .ToList();*/

/*if (candidates.Count == 0)
{
    throw new InvalidOperationException("No defined ConnectionStrings");
}*/

// Choosing the first working connection string
//var chosen = await ChooseFirstWorkingAsync(candidates);
//Console.WriteLine($"[DB] Chosen ConnectionString: {chosen.Name}");

// Ensure the ApplicationDbContext is registered as a service
builder.Services.AddDbContext<MyDbContext>(conf =>
    conf.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")) /*o => o.EnableRetryOnFailure(maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null))*/);
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

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1",new OpenApiInfo{Title = "API engineer thesis", Version = "v1"
        , Description = "API for Employee management application"});
    //option.OperationFilter<FileUploadOperationFilter>();
    
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
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),@"wwwroot")),
    RequestPath = new PathString("/Resources")
});

app.UseRouting();

app.UseCors("AllowAngularDevServer");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
