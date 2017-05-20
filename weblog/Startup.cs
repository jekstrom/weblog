using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using weblog.Services;
using weblog.Repositories;
using weblog.Models;
using weblog.Data;

namespace weblog
{
    public class Startup
    {
		private readonly string _connectionString;
		private readonly IHostingEnvironment _env;

		public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

			if (env.IsDevelopment())
			{
				builder.AddUserSecrets<Startup>();
			}

			Configuration = builder.Build();

			_connectionString = Configuration["StorageConnectionString"];
			_env = env;
		}

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
			{
				if (_env.IsDevelopment())
				{
					options.SslPort = 44301;
				} else
				{
					options.SslPort = 443;
				}
				options.Filters.Add(new RequireHttpsAttribute());
			});

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(Configuration["weblogsql2"]));

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddTransient<IPostRepository, PostRepository>(s => new PostRepository(_connectionString));
			services.AddTransient<IPostService>(s => new PostService(s.GetService<IPostRepository>()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

			app.UseIdentity();

			app.UseGoogleAuthentication(
				new GoogleOptions
				{
					ClientId = Configuration["Authentication:Google:ClientId"],
					ClientSecret = Configuration["Authentication:Google:ClientSecret"]
				}
			);

			app.UseMvc(routes =>
            {
				routes.MapRoute(
					name: "posts",
					template: "{controller=Posts}/{action=Post}/{postName}"
				);
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
				);
            });
        }
    }
}
