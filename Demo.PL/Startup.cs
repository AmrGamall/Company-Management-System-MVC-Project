using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Demo.PL.ConfigSetting;
using Demo.PL.Helpers;
using Demo.PL.MappingProfiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Demo.PL
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddDbContext<MVCAppDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			// Allow Dependency Injection
			services.AddScoped<IDepartmentRepository, Department_Repository>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			// AutoMapper configuration
			services.AddAutoMapper(cfg => cfg.AddProfiles(new List<Profile>
			{
				new EmployeeProfile(),
				new UserProfile(),
				new RoleProfile()
			}));

			// Identity configuration
			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = true;
			})
			.AddEntityFrameworkStores<MVCAppDbContext>()
			.AddDefaultTokenProviders();

			// Cookie Authentication configuration
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
					.AddCookie(options =>
					{
						options.LoginPath = "/Account/Login"; // Ensure paths start with a '/'
						options.AccessDeniedPath = "/Home/Error";
					});

			// Email configuration
			services.Configure<EmailConfigSetting>(Configuration.GetSection("NewEmailSettings"));
			services.AddScoped<INewEmailSettings, NewEmailSettings>();

			// Google Authentication configuration
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			})
			.AddGoogle(options =>
			{
				IConfigurationSection googleAuthSection = Configuration.GetSection("Authentication:Google");
				options.ClientId = googleAuthSection["ClientId"];
				options.ClientSecret = googleAuthSection["ClientSecret"];
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Account}/{action=Login}/{id?}");
			});
		}
	}
}

