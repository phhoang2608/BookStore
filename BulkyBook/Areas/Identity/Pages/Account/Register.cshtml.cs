﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Respository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BulkyBook.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty] //binded the InputModel to be used in Register Razor Pages
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
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
            public string Name { get; set; }
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string PhoneNumber { get; set; }
            public int? CompanyId { get; set; }
            public string Role { get; set; }

            public IEnumerable<SelectListItem> CompanyList { get; set; }
            public IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            //inject into drop down if user is logged in as Admin
            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                //retrieve list to our drop down and pass this to our RazorPage
                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_User_Indi).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
            };
            if(User.IsInRole(SD.Role_Employee))
            {
                Input.RoleList = _roleManager.Roles.Where(u => u.Name == SD.Role_User_Comp).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                });
            }
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                //we prefer to use our class Application User
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    CompanyId = Input.CompanyId,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    State = Input.State,
                    PostalCode = Input.PostalCode,
                    Name = Input.Name,
                    PhoneNumber = Input.PhoneNumber,
                    Role = Input.Role
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if(!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Employee))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User_Comp))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User_Indi))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi));
                    }

                    if(user.Role == null)
                    {
                        //if user was not assigned any role, assigned individual user role
                        await _userManager.AddToRoleAsync(user, SD.Role_User_Indi);
                    }
                    else
                    {
                        //if user company ID is greater than 0 then we will assign user to a user company role
                        if(user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_User_Comp);
                        }
                        await _userManager.AddToRoleAsync(user,user.Role);
                    }
                    //await _userManager.AddToRoleAsync(user, SD.Role_Admin);
                    //for right now anyone who registered will be granted admin by default
                    //have to delete or comment this out later on.


                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    var PathToFile = _hostEnvironment.WebRootPath + Path.DirectorySeparatorChar.
                        ToString()+"Templates"+ Path.DirectorySeparatorChar.
                        ToString()+"EmailTemplates" + Path.DirectorySeparatorChar.
                        ToString()+ 
                        "Confirm_Account_Registration.html";

                    var subject = "Confirm Account Registration";
                    string HtmlBody = "";
                    using (StreamReader streamReader = System.IO.File.OpenText(PathToFile))
                    {
                        HtmlBody = streamReader.ReadToEnd();
                    }

                    //{0} : Subject
                    //{1} : DateTime
                    //{2} : Name
                    //{3} : Email
                    //{4} : Message
                    //{5} : callbackUrl

                    string Message = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";

                    string messageBody = string.Format(HtmlBody,
                        subject,
                        String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                        user.Name,
                        user.Email,
                        Message,
                        callbackUrl);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        messageBody);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        //if user creating a new account
                        if (user.Role == null)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            //admin is creating a new user so we don't want to log him/her out 
                            //and sign in as the newly created user so we redirect him/her to list
                            //of users
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                //retrieve list to our drop down and pass this to our RazorPage
                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_User_Indi).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
            };

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
