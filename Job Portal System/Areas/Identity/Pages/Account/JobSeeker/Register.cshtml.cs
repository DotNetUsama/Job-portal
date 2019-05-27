using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Job_Portal_System.Areas.Identity.Pages.Account.JobSeeker
{
    [AllowAnonymous]
    public class RegisterAsJobSeekerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterAsJobSeekerModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterAsJobSeekerModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterAsJobSeekerModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            [StringLength(25)]
            [DataType(DataType.Text)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            [DataType(DataType.Text)]
            [StringLength(25)]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Gender")]
            [EnumDataType(typeof(GenderType))]
            public GenderType Gender { get; set; }

            [Required]
            [Display(Name = "Birth date")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "State")]
            public string State { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [StringLength(255)]
            [Display(Name = "Detailed address")]
            public string DetailedAddress { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid) return Page();
            var user = new User
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = (byte)Input.Gender,
                BirthDate = Input.BirthDate,
                PhoneNumber = Input.PhoneNumber,
                CityId = Input.City,
                DetailedAddress = Input.DetailedAddress,
            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                result = await _userManager.AddToRoleAsync(user, "JobSeeker");
                if (result.Succeeded)
                {
                    CreateJobSeeker(user);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    _context.SaveChangesAsync().Wait();
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Models.JobSeeker CreateJobSeeker(User user)
        {
            var jobSeeker = _context.JobSeekers.Add(new Models.JobSeeker
            {
                UserId = user.Id,
            });
            return jobSeeker.Entity;
        }
    }
}
