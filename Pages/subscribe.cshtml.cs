using AIStudyPlanner.Data;
using AIStudyPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AIStudyPlanner.Pages
{
    public class SubscribeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public SubscribeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync(string email)
        {
            // Empty email
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToPage("/Index");
            }

            email = email.Trim();

            // Validate email
            var validator = new EmailAddressAttribute();

            if (!validator.IsValid(email))
            {
                return RedirectToPage("/Index");
            }

            // Check whether email already exists
            bool alreadyExists = _context.EmailSubscribers
                .Any(x => x.Email.ToLower() == email.ToLower());

            if (!alreadyExists)
            {
                _context.EmailSubscribers.Add(new EmailSubscriber
                {
                    Email = email,
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
            }

            TempData["SubscribeMessage"] =
                "Thank you We will contact you ❤️";

            return RedirectToPage("/Index");
        }
    }
}