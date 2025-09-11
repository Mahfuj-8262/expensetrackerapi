using ExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// EF Core config
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT setup
var jwtConfig = builder.Configuration.GetSection("Jwt");
var secretKey = jwtConfig["Key"] ?? throw new InvalidOperationException("JWT Key is missing!");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("https://blue-flower-0ee107200.1.azurestaticapps.net") 
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;  // allow http for local dev
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


// using ExpenseTracker.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();

// // EF Core database configuration
// builder.Services.AddDbContext<AppDbContext>(opts =>
//     opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// // JWT Configuration
// var jwtConfig = builder.Configuration.GetSection("Jwt");
// var secretKey = jwtConfig["Key"] ?? throw new InvalidOperationException("JWT Key is missing!");

// // Add Authentication and JWT Bearer
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = jwtConfig["Issuer"],
//         ValidAudience = jwtConfig["Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
//         ClockSkew = TimeSpan.Zero
//     };

//     options.Events = new JwtBearerEvents
//     {
//         OnAuthenticationFailed = context =>
//         {
//             var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
//             logger.LogError(context.Exception, "JWT authentication failed!");
//             return Task.CompletedTask;
//         },
//         OnTokenValidated = context =>
//         {
//             var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
//             logger.LogInformation("JWT token validated for user: {UserId}", 
//                 context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
//             return Task.CompletedTask;
//         },
//         OnChallenge = context =>
//         {
//             var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
//             logger.LogWarning("JWT challenge triggered. Authentication failed!");
//             return Task.CompletedTask;
//         }
//     };
// });

// // Authorization services
// builder.Services.AddAuthorization();

// // Swagger + JWT Authorization Configuration
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense Tracker API", Version = "v1" });

//     // Enable JWT Bearer token in Swagger UI
//     var securityScheme = new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         Scheme = "bearer",
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "Enter JWT token like: Bearer {your token}"
//     };

//     c.AddSecurityDefinition("Bearer", securityScheme);

//     var securityRequirement = new OpenApiSecurityRequirement
//     {
//         { securityScheme, new[] { "Bearer" } }
//     };

//     c.AddSecurityRequirement(securityRequirement);
// });

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// // Authentication + Authorization
// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();

// app.Run();