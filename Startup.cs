using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using freezebee_api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using freezebee_api.Services;
using Microsoft.AspNetCore.Http;
using freezebee_api.Middlewares;

namespace freezebee_api
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
            string connectedString = Configuration.GetConnectionString("Freezebee");
            var server = Configuration["DBServer"] ?? "10.10.40.13";
            var port = Configuration["DBPort"] ?? "1433";
            var username = Configuration["DBUsername"] ?? "root";
            var password = Configuration["DBPassword"] ?? "1AH5Dk33c7bWERl";
            var database = Configuration["DBDatabase"] ?? "Freezebee";

            var key = Encoding.ASCII.GetBytes(TokenService.Secret);

            services.AddDbContext<FreezebeeContext>(options =>
            {
                options.UseSqlServer($"Server={server};Database={database}; User Id={username}; Password={password};");
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins("http://10.10.40.16:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.RequireHttpsMetadata = false;
                      options.SaveToken = true;
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey = new SymmetricSecurityKey(key),
                          ValidateIssuer = false,
                          ValidateAudience = false,
                          ClockSkew = TimeSpan.Zero
                      };
                      options.Events = new JwtBearerEvents
                      {
                          OnMessageReceived = context =>
                          {
                              context.Token = context.Request.Cookies["freezebee_session"];
                              return Task.CompletedTask;
                          }
                      };
                  });


            services.AddControllers()
                    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "freezebee_api", Version = "v1" });
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //     app.UseSwagger();
                //     app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "freezebee_api v1"));
            }
            app.UseMiddleware<EncryptMiddleware>();

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
