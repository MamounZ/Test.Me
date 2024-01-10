using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Test.Me.models;

namespace Test.Me.Pages.Teachers
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _context;

        public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string Temail, string Tpassword)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(Temail) || string.IsNullOrWhiteSpace(Tpassword))
                {
                    ModelState.AddModelError("", "Email and password are required.");
                    return Page();
                }

                // Hash the entered password
                var hashedPassword = HashPassword(Tpassword);

                // Check teacher credentials against the database using Entity Framework Core
                var teacher = _context.Teachers.FirstOrDefault(t => t.Temail == Temail && t.Tpassword == hashedPassword);

                if (teacher != null)
                {
                    // Successful login, set session variable for Teacher ID
                    HttpContext.Session.SetInt32("TeacherId", teacher.Tid);

                    // Log to check if it reaches this point
                    _logger.LogInformation("Teacher login successful");

                    // Redirect to the program or dashboard
                    return RedirectToPage("/Teachers/ExamList");
                }

                // Log to check if it reaches this point
                _logger.LogInformation("Invalid credentials");

                // Invalid credentials
                ModelState.AddModelError("", "Invalid email or password.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during teacher login.");
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
                return Page();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
