using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Test.Me.models;

namespace Test.Me.Pages.Students
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterModel(IConfiguration configuration, ILogger<RegisterModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string Sname, string Semail, string Spassword)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrWhiteSpace(Sname) || string.IsNullOrWhiteSpace(Semail) || string.IsNullOrWhiteSpace(Spassword))
                {
                    ModelState.AddModelError("", "Name, email, and password are required.");
                    return Page();
                }

                // Check if the email is already in use
                if (_context.Students.Any(s => s.Semail == Semail))
                {
                    ModelState.AddModelError("", "Email is already in use.");
                    return Page();
                }

                // Hash the password
                var hashedPassword = HashPassword(Spassword);

                // Store student information in the database
                var newStudent = new Student
                {
                    Sname = Sname,
                    Semail = Semail,
                    Spassword = hashedPassword
                };

                _context.Students.Add(newStudent);
                _context.SaveChanges();

                return RedirectToPage("/Students/Login"); // Redirect to the login page after successful registration
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during student registration.");
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
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
