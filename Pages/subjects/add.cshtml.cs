using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AIStudyPlanner.Pages.Subjects
{
    [Authorize]
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Subject Subject { get; set; } = new Subject();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return Challenge();
            }

            Subject.UserId = currentUserId;

            _context.Subjects.Add(Subject);

            await _context.SaveChangesAsync();

            TempData["DashboardMessage"] =
                "📚 Subject added successfully!";

            return RedirectToPage("/Dashboard");
        }
    }
}