using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using weblog.Services;
using weblog.Repositories;
using weblog.Models;
using weblog.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Amazon.S3;
using Amazon;
using Serilog;
using System.IO;
using Polly;
using System;

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

			Log.Logger = new LoggerConfiguration()
			   .MinimumLevel.Debug()
			   .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "log-{Date}.txt"))
			   .CreateLogger();


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
				options.UseNpgsql(Configuration["postgres"]));

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(o => o.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"))
				.AddGoogle(o =>
				{
					o.ClientId = Configuration["Authentication:Google:ClientId"];
					o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
				}
			);

			IAmazonS3 client = new AmazonS3Client(new Amazon.Runtime.BasicAWSCredentials(Configuration["aws_client_access_key"], Configuration["aws_secret_access_key"]), RegionEndpoint.USEast1);

			AwsConfiguration config = new AwsConfiguration();
			Configuration.GetSection("AWS").Bind(config);

			IAsyncPolicy retryPolicy = Policy
			  .Handle<Exception>()
			  .WaitAndRetryAsync(5, retryAttempt =>
				TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
				onRetry: (exception, retryTimespan) => Log.Logger.Error(exception, $"Retry count {retryTimespan}")
			  );

			IAsyncPolicy bulkheadPolicy = Policy.BulkheadAsync(
				10, 
				onBulkheadRejectedAsync: async (context) => Log.Logger.Error("Bulkhead Rejection")
				
			);

			IAsyncPolicy policy = Policy.WrapAsync(retryPolicy, bulkheadPolicy);

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
			services.AddTransient<IPostRepository, S3PostRepository>(s => new S3PostRepository(client, config.BucketName, config.KeyPrefix, Log.Logger, policy));
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

			app.UseAuthentication();

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
