using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Movies.API.Contexts;
using Movies.API.Services;


namespace Movies.API
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
            services.AddControllers(options =>
            {
                // 406 when requested unsupported media type
                options.ReturnHttpNotAcceptable = true;

                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options));

                // Set XML as default format instead of JSON - the first formatter in the 
                // list is the default, so we insert the input/output formatters at 
                // position 0
                //options.OutputFormatters.Insert(0,new XmlSerializerOutputFormatter());
                //options.InputFormatters.Insert(0, new XmlSerializerInputFormatter(options));
            });
            // compressing responses support eg gzip
            services.AddResponseCompression();
            // suppress automatic 400 provided by ApiController attr in favor of 422 Unprocessable Entity
            // when validation fails
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            string connectionString = Configuration.GetConnectionString("MoviesDBConnectionString");
            services.AddDbContext<MoviesContext>(o => o.UseSqlServer(connectionString));

            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<IPostersRepository, PostersRepository>();
            services.AddScoped<ITrailersRepository, TrailersRepository>();

            services.AddAutoMapper();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Accept-Encoding on client
            app.UseResponseCompression();

            app.UseSwagger();

            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("swagger/v1/swagger.json", "Movies.API v1");
                o.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthorization();
      
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
