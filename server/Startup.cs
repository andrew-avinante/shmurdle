using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using server.Models;
using server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "server", Version = "v1" });
            });

            // requires using Microsoft.Extensions.Options
            services.Configure<WordDatabaseSettings>(
                Configuration.GetSection(nameof(WordDatabaseSettings)));

            services.AddSingleton<IDbSettings>(sp =>
                sp.GetRequiredService<IOptions<WordDatabaseSettings>>().Value);

            services.AddSingleton<WordsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(
                    options => options.WithOrigins("localhost:8080").AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader()
                );
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "server v1"));
            } else
            {
                app.UseCors(
                    options => options.WithOrigins("shmurdle.andrewavinante.com").AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader()
                );
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
