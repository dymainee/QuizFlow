using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace QuizFlow.Infrastructure.Authentication;

//Пропустить запрос, если токен подписан именно твоим секретным ключом
public static class AuthExtensions
{

    // отдаеm его встроенному сервисуJwtBearerHandler.
    public static IServiceCollection addAuth(this IServiceCollection services, IConfiguration configuration)  //// <-- 'this' делает из метода "расширение"
    { //nameof возвращает имя переменной, типа или члена класса в виде обычной строки
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        //схема по умолчанию — JwtBearer
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //обработчик JWT-токенов
            .AddJwtBearer(options =>
            { //параметры валидации
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    //симметричный ключ, с помощью которого проверяется подпись.
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };




                options.Events = new JwtBearerEvents
                {
                    //JwtBearerEvents — это класс-перехватчик (событийная модель) в ASP.NET Core,
                    //который позволяет вклиниться в процесс аутентификации
                    //по JWT-токену на разных этапах его обработки.

                    OnMessageReceived = new Func<MessageReceivedContext, Task>(context =>
                    {/// Проверяем, есть ли в Cookie наш токен с именем "jwt"
                        if (context.Request.Cookies.ContainsKey("jwt"))
                        { //берёт строку токена из куки и записывает её в пол
                            context.Token = context.Request.Cookies["jwt"];
                        }
                        //уже выполненная (завершённая) задача.
                        return Task.CompletedTask;
                    })
                };
            });
        return services;

    }
}
//Берёт объект options.

//Запускает событие OnMessageReceived и ждёт, пока ты запишешь токен в context.Token.

//Сразу после этого он сам считывает значение из context.Token.

//Берет options.TokenValidationParameters из этого же самого options и вызывает функцию проверки!
