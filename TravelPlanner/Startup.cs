using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using TravelPlanner.Filters;
using TravelPlanner.Filters.OAuth;
using TravelPlanner.Infrastructure;

[assembly: OwinStartup(typeof(TravelPlanner.Startup))]

namespace TravelPlanner
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            WebApiConfig.Register(config);
            config.Filters.Add(new CheckModelForNullAttribute());
            config.Filters.Add(new ValidateModelStateAttribute());
            FluentValidationModelValidatorProvider.Configure(config);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();

            builder
              .RegisterType<ApplicationOAuthProvider>()
              .As<IOAuthAuthorizationServerProvider>()
              .PropertiesAutowired() // to automatically resolve IUserService
              .SingleInstance(); // you only need one instance of this provider

            // Repo
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces().InstancePerRequest();

            // Manager
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Manager"))
                .AsImplementedInterfaces().InstancePerRequest();

            // Identity managers
            builder.Register(
                c =>
                {
                    var usrMgr =
                        new UserManager<IdentityUser>(new UserStore<IdentityUser>(c.Resolve<IDbFactory>().Init()));

                    // Configure validation logic for usernames
                    usrMgr.UserValidator = new UserValidator<IdentityUser>(usrMgr)
                    {
                        AllowOnlyAlphanumericUserNames = false,
                        RequireUniqueEmail = true
                    };

                    // Configure validation logic for passwords
                    usrMgr.PasswordValidator = new PasswordValidator
                    {
                        RequiredLength = 6,
                        RequireNonLetterOrDigit = true,
                        RequireDigit = true,
                        RequireLowercase = true,
                        RequireUppercase = true,
                    };
                    return usrMgr;
                })
                .InstancePerRequest();

            builder.Register(
                c => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(c.Resolve<IDbFactory>().Init())))
                .InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            //app.UseAutofacMvc();
            ConfigureOauth(app, container);
            app.UseWebApi(config);
        }

        private void ConfigureOauth(IAppBuilder app, IContainer container)
        {
            // Configure the application for OAuth based flow
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = container.Resolve<IOAuthAuthorizationServerProvider>(new NamedParameter("publicClientId", "travelplanner")),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}
