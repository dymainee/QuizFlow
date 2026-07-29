using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizFlow.Application.Interfaces;
using QuizFlow.Application.Services;
using QuizFlow.Data;
using QuizFlow.Infrastructure.Authentication;
using QuizFlow.Infrastructure.Interfaces;
using QuizFlow.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
//builder.Services
//Это «коллекция служб» (DI-контейнер) в .NET. Все сервисы, настройки,
//репозитории и конфигурации безопасности регистрируются чтобы приложение знало, как их создавать и использовать.

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
//нет готовых объектов, мы передаем контейнеру DI саму информацию о типах.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IHashService, HashService>();
//builder.Configuration — это объект, который содержит все настройки приложения из appsettings.json
//Чтобы валидировать входящие JWT-токены
builder.Services.addAuth(builder.Configuration);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
