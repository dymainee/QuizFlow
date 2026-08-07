using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuizFlow.Application.Interfaces;
using QuizFlow.Application.Services;
using QuizFlow.DTO;
using QuizFlow.Models.Enums;
using System.Security.Claims;

namespace QuizFlow.Controllers
    //
{ // FromBody почему делают record dto 
    // можно ли ДТО в Сервисы 
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
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
                var token = await _userService.LoginAsync(dto);
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
                await _userService.RegisterAsync(dto);
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
                await _userService.RegisterAsync(dto);
                return RedirectToAction("Login");
            }
            catch (Exception ex) {
                return View(dto);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ShowStudentProfile(StudentProfileDTO dto, string selectedSort)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(id)) return Unauthorized();
            Guid userId = Guid.Parse(id);
            if (!string.IsNullOrEmpty(selectedSort))
            {
                var parts = selectedSort.Split('_');
                dto.universalDTO.sortField = parts[0];
                dto.universalDTO.sortOrder = parts[1] == "Asc" ? SortOrder.Ascending : SortOrder.Descending;

            }
            StudentProfileDTO ouputdto = await _userService.GetStudentProfileAsync(userId, dto);
            return View(ouputdto);

        }
        [HttpPost]
        public async Task<IActionResult> ShowStudentProfile(StudentProfileDTO dto) {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(id);
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _userService.UpdateStudentProfile(userId, dto);
            return RedirectToAction("ShowStudentProfile");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ShowTeacherProfile(TeacherProfileDTO dto, string selectedSort)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(id);
            if (string.IsNullOrEmpty(id)) return Unauthorized();
            dto.universalDTO ??= new UniversalDTO();
            if (!string.IsNullOrEmpty(selectedSort))
            {
                var parts = selectedSort.Split('_');
                dto.universalDTO.sortField = parts[0];
                dto.universalDTO.sortOrder = parts[1] == "Asc" ? SortOrder.Ascending : SortOrder.Descending;

            }
            TeacherProfileDTO ouputdto = await _userService.GetTeacherProfileAsync(userId, dto);
            return View(ouputdto);
        }

        [HttpPost]
        public async Task<IActionResult> ShowTeacherProfile(TeacherProfileDTO dto)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(id);
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            await _userService.UpdateTeacherProfile(userId,dto);
            return RedirectToAction("ShowTeacherProfile");
        }


        public IActionResult ReturnToMenu()
        {
            return RedirectToAction("ShowAll", "Menu");
        }





    }
}
