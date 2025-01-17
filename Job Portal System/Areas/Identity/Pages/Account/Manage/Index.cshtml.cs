﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Job_Portal_System.Data;
using Job_Portal_System.Enums;
using Job_Portal_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Job_Portal_System.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }
        public string ImageUrl { get; set; }

        public UserType UserType { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public SelectList States { get; set; }
        public SelectList Cities { get; set; }

        public class InputModel
        {
            [HiddenInput]
            [Display(Name = "Profile picture")]
            [DataType(DataType.ImageUrl)]
            public string Image { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

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

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var city = _context.Cities.SingleOrDefault(c => c.Id == user.CityId);

            UserType = User.IsInRole("JobSeeker") ? UserType.JobSeeker :
                User.IsInRole("Recruiter") ? UserType.Recruiter :
                User.IsInRole("Administrator") ? UserType.Administrator :
                UserType.Other;

            Username = userName;
            ImageUrl = user.Image ?? "https://i.imgur.com/uJ3eXxp.png";

            States = new SelectList(_context.States.ToDictionary(s => s.Id, s => s.Name), "Key", "Value");
            if (city != null)
            {
                Cities = new SelectList(_context.Cities
                    .Where(c => c.StateId == city.StateId)
                    .ToDictionary(c => c.Id, c => c.Name), "Key", "Value");
            }

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                DetailedAddress = user.DetailedAddress,
                City = city?.Id,
                State= city?.StateId,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            if (Input.City != user.CityId || Input.DetailedAddress != user.DetailedAddress)
            {
                user.Image = Input.Image;
                user.CityId = Input.City;
                user.DetailedAddress = Input.DetailedAddress;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting address information for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
