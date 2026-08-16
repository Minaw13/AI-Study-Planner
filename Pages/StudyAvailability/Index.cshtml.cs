using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AIStudyPlanner.Pages.StudyAvailability
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Dictionary<DayOfWeek, double> Hours { get; set; }
            = new Dictionary<DayOfWeek, double>();

        public double TotalWeeklyHours { get; set; }

        public async Task OnGetAsync()
        {
            await LoadAvailabilityAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            if (Hours.Any(x => x.Value < 0 || x.Value > 24))
            {
                ModelState.AddModelError(
                    "",
                    "Available hours must be between 0 and 24."
                );

                CalculateTotal();
                return Page();
            }

            var existingAvailability = await _context.StudyAvailabilities
                .Where(a => a.UserId == userId)
                .ToListAsync();

            _context.StudyAvailabilities.RemoveRange(
                existingAvailability
            );

            foreach (var day in Enum.GetValues<DayOfWeek>())
            {
                var hours = Hours.ContainsKey(day)
                    ? Hours[day]
                    : 0;

                _context.StudyAvailabilities.Add(
                    new AIStudyPlanner.Models.StudyAvailability
                    {
                        UserId = userId,
                        DayOfWeek = day,
                        AvailableHours = hours
                    }
                );
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Dashboard");
        }

        private async Task LoadAvailabilityAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return;
            }

            var availability = await _context.StudyAvailabilities
                .Where(a => a.UserId == userId)
                .ToListAsync();

            Hours = Enum
                .GetValues<DayOfWeek>()
                .ToDictionary(
                    day => day,
                    day => availability
                        .FirstOrDefault(a => a.DayOfWeek == day)
                        ?.AvailableHours ?? 0
                );

            CalculateTotal();
        }

        private void CalculateTotal()
        {
            TotalWeeklyHours = Hours.Values.Sum();
        }
    }
}