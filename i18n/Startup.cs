using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Localization.Routing;
using i18n.Localization;
using i18n.Middlewares;
using Microsoft.AspNetCore.Rewrite;
using System;
using Microsoft.AspNetCore.Http;

namespace i18n
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }
        public static List<CultureInfo> SupportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-GB"),
            new CultureInfo("en-US")
        };
        public static RequestCulture DefaultRequestCulture = new RequestCulture(SupportedCultures[0].Name);

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            Configuration = builder.Build();
        }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddSingleton(Configuration);

            services.AddRazorPages(options =>
            {
                options.Conventions.Add(new CustomCultureRouteModelConvention());
            }).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddHttpContextAccessor();
                

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = DefaultRequestCulture;
                options.SupportedCultures = SupportedCultures;
                options.SupportedUICultures = SupportedCultures;
                options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider { Options = options });
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
                //options.AppendTrailingSlash = true;
            });

            services.AddTransient<RedirectUnsupportedCultures>();

            services.AddOptions();
            services.AddLogging();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseRequestLocalization();
            //app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();

            // Attempt to make auto-redirect to culture if it is not exist in the url
            //app.UseMiddleware<RedirectUnsupportedCultures>();
            RewriteOptions rewriter = new RewriteOptions();
            rewriter.Add(new RedirectUnsupportedCultures(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()));
            app.UseRewriter(rewriter);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
