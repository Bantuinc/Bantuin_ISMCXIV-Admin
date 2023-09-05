using Bantuin_ISMCXIV_Admin;
using Bantuin_ISMCXIV_Admin.Data;
using Bantuin_ISMCXIV_Admin.Helpers;
using Bantuin_ISMCXIV_Admin.Interfaces;
using Bantuin_ISMCXIV_Admin.Models;
using Bantuin_ISMCXIV_Admin.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ResponseHelper>();
builder.Services.AddTransient<Seed>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bantuin_ISMCXIV", Version = "v1" });

	// Konfigurasi untuk menambahkan header Bearer
	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		Description = "Enter 'token' for authorization"
	};
	c.AddSecurityDefinition("Bearer", securityScheme);

	var securityRequirement = new OpenApiSecurityRequirement
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
			new string[] {}
		}
	};
	c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddDbContext<DataContext>(options => {
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.User.RequireUniqueEmail = true;
	options.SignIn.RequireConfirmedEmail = true;
})
	.AddEntityFrameworkStores<DataContext>()
	.AddDefaultTokenProviders();
builder.Services.AddAuthentication()
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Authentication:Schemes:Bearer:SigningKeys:0:Value").Value!))
		};
		options.Events = new JwtBearerEvents
		{
			OnChallenge = async context =>
			{
				context.HandleResponse();
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				context.Response.ContentType = "application/json";
				var result = new UnauthorizedObjectResult(new ResponseHelper().Error("You are not authorized", 401));
				await result.ExecuteResultAsync(new ActionContext
				{
					HttpContext = context.HttpContext
				});
			},
			OnForbidden = async context =>
			{
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				context.Response.ContentType = "application/json";
				var result = new ObjectResult(new ResponseHelper().Error("You are forbidden to access this resource", 403))
				{
					StatusCode = (int)HttpStatusCode.Forbidden
				};
				await result.ExecuteResultAsync(new ActionContext
				{
					HttpContext = context.HttpContext
				});
			}
		};
	});

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
	SeedData(app);

async void SeedData(IHost app)
{
	var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

	using (var scope = scopedFactory!.CreateScope())
	{
		var service = scope.ServiceProvider.GetService<Seed>();
		await service!.SeedDataContextAsync();
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
