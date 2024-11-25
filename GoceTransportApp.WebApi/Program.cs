
using GoceTransportApp.Data.Common.Repositories;
using GoceTransportApp.Data.Common;
using GoceTransportApp.Data.Repositories;
using GoceTransportApp.Data;
using GoceTransportApp.Services.Data.Cities;
using GoceTransportApp.Services.Data.Streets;
using Microsoft.EntityFrameworkCore;

namespace GoceTransportApp.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
            string? goceTransportAppOrigins = builder.Configuration.GetValue<string>("Client Origins:GoceTransportApp");

            // Add services to the container.

            builder.Services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Data repositories
            builder.Services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            //Application services
            builder.Services.AddScoped<IStreetService, StreetService>();
            builder.Services.AddScoped<ICityService, CityService>();

            //TODO: Allow CORS!!!
            builder.Services.AddCors(cfg =>
            {
                cfg.AddPolicy("AllowAll", policyBld =>
                {
                    policyBld
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });

                if (!String.IsNullOrWhiteSpace(goceTransportAppOrigins))
                {
                    cfg.AddPolicy("AllowMyServer", policyBld =>
                    {
                        policyBld
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithOrigins(goceTransportAppOrigins);
                    });
                }
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            if (!String.IsNullOrWhiteSpace(goceTransportAppOrigins))
            {
                app.UseCors("AllowMyServer");
            }

            app.MapControllers();

            app.Run();
        }
    }
}
