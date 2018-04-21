using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NgKillerApiCore.DAL;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json;
using NgKillerApiCore.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NgKillerApiCore.Hubs;

namespace NgKillerApiCore
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<KillerContext>(opt => opt.UseInMemoryDatabase("Killer"));
            services.AddDbContext<KillerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b=>b.MigrationsAssembly("NgKillerApiCore")));

            services.AddSignalR();
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling =
                                           Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //Préserver la capitalisation des noms de variable lors de la sérialisation
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //Add Cors support to the service
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", corsBuilder.Build());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "KillerAPI", Version = "v1" });
                c.IncludeXmlComments($@"{System.AppDomain.CurrentDomain.BaseDirectory}\NgKillerApiCore.xml");
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="serviceProvider"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(); 
            app.UseCors("AllowAll");
            app.UseSignalR(routes =>
            {
                routes.MapHub<RequestHub>("/requesthub");
            });
            app.UseWebSockets();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "KillerAPI V1");
            });
        } 
    }
    
}
