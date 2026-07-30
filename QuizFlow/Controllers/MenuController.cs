using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Models;
using System.Security.Claims;

namespace QuizFlow.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        
        [HttpGet]
        public async Task<IActionResult> ShowAll(MenuQuizShowDTO dto) {
            var quizzes = await _menuService.GetAllAsync(dto);
            return View(quizzes);
        }


    }
}
