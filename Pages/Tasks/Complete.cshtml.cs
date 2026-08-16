using AIStudyPlanner.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Pages.Tasks
{
    [Authorize]
    public class CompleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CompleteModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                return Challenge();
            }

            var task = await _context.StudyTasks
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    t.UserId == currentUserId);

            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = !task.IsCompleted;

            await _context.SaveChangesAsync();

            if (task.IsCompleted)
            {
                TempData["TaskMessage"] = "Task completed successfully! ✅";
            }
            else
            {
                TempData["TaskMessage"] = "Task marked as incomplete. ↩️";
            }

            return RedirectToPage("/Tasks");
        }
    }
}