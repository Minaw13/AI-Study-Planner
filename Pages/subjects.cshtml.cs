using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Pages
{
    [Authorize]
    public class SubjectsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubjectsModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Subject> Subjects { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return;
            }

            Subjects = await _context.Subjects
                .Where(s => s.UserId == currentUserId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}