using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Models.Enums;

namespace QuizFlow.Controllers
    //
{ // FromBody почему делают record dto 
    // можно ли ДТО в Сервисы 
    public class UserController : Controller
    {
        private readonly IAuthService _authService;
        public UserController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {   
                var token = await _authService.LoginAsync(dto);
                //сервер отправляет обратно
                Response.Cookies.Append("jwt", token);
                return RedirectToAction("ShowAll", "Menu");
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult RegisterStudent()
        {
            return View();
        }
        public async Task<IActionResult> RegisterStudent(RegisterStudentDto dto) {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                await _authService.RegisterAsync(dto);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return View(dto);
            }
        }

        [HttpGet]
        public IActionResult RegisterTeacher() {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RegisterTeacher(RegisterTeacherDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                await _authService.RegisterAsync(dto);
                return RedirectToAction("Login");
            }
            catch (Exception ex) {
                return View(dto);
            }
        }

    }
}
