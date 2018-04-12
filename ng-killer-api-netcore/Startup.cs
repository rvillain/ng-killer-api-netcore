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
using WebSocketManager;

namespace NgKillerApiCore
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
            services.AddDbContext<KillerContext>(opt => opt.UseInMemoryDatabase("Killer"), ServiceLifetime.Singleton);
            services.AddSingleton(typeof(SocketManager));
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling =
                                           Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //Préserver la capitalisation des noms de variable lors de la sérialisation
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            }); ;

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
            });


            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(); 
            app.UseCors("AllowAll");

            app.UseWebSockets();
            //app.Use(async (context, next) =>
            //    {
            //        if (context.Request.Path == "/ws")
            //        {
            //            if (context.WebSockets.IsWebSocketRequest)
            //            {
            //                try
            //                {
            //                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //                    SocketManager service = serviceProvider.GetService<SocketManager>();
            //                    await service.ManageSocket(context, webSocket);
            //                }
            //                catch(Exception ex)
            //                {

            //                }
            //            }
            //            else
            //            {
            //                context.Response.StatusCode = 400;
            //            }
            //        }
            //        else
            //        {
            //            await next();
            //        }

            //    });

            app.MapWebSocketManager("/ws", serviceProvider.GetService<MessageHandler>());

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "KillerAPI V1");
            });
        } 
    }
    
}
