using Microsoft.Data.SqlClient;
using Repertory.Repositories;
using Repertory.Services;

namespace Repertory {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddScoped(sql => new SqlConnection(builder.Configuration.GetConnectionString("ProductionConnection")));

            builder.Services.AddTransient<CategoryService>();
            builder.Services.AddTransient<CategoryRepository>();

            builder.Services.AddTransient<GroupService>();
            builder.Services.AddTransient<GroupRepository>();

            builder.Services.AddTransient<SheetMusicService>();
            builder.Services.AddTransient<SheetMusicRepository>();

            builder.Services.AddTransient<InstrumentService>();
            builder.Services.AddTransient<InstrumentRepository>();

            builder.Services.AddTransient<TeachingLiteratureService>();
            builder.Services.AddTransient<TeachingLiteratureRepository>();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.MapControllers();

            app.Run();
        }
    }
}