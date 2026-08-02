using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using QuizFlow.Application.Interfaces;
using QuizFlow.DTO;
using QuizFlow.Models;
using QuizFlow.Models.Enums;
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
        public async Task<IActionResult> ShowAll(MenuQuizShowDTO dto, string selectedSort) {
            if (!string.IsNullOrEmpty(selectedSort)) {
                var parts = selectedSort.Split('_');
                dto.sortField = parts[0];
                dto.sortOrder = parts[1] == "Asc" ? SortOrder.Ascending : SortOrder.Descending;

            }
            var quizzes = await _menuService.GetAllAsync(dto);
            return View(quizzes);
        }


    }
}
