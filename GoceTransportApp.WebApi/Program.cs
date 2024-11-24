
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
