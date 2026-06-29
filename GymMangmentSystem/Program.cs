using GymMangmentSystem.BLL.Helpers;
using AutoMapper;
using GymMangmentSystem.BLL.Services.Classes;
using GymMangmentSystem.BLL.Services.Interfaces;
using GymMangmentSystem.DAL.Data.DbContexts;
using GymMangmentSystem.DAL.Repositories.Classes;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace GymMangmentSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //builder.Services.AddScoped<IPlanRepository , PlanRepository>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //for genaric

            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

            builder.Services.AddScoped<ISessionRepository, SessionRepository>();

            builder.Services.AddScoped<ISessionService, SessionService>();

            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));
            //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
