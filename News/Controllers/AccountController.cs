using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News.DbModels;
using News.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace News.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly NewsDbContext _newsDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, NewsDbContext newsDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _newsDbContext = newsDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            foreach (var key in ModelState.Keys)
            {
                var entry = ModelState[key];
                if (entry.Errors.Any())
                {
                    foreach (var error in entry.Errors)
                    {
                        // You can log or print the validation error messages here
                        Console.WriteLine($"Property: {key}, Error: {error.ErrorMessage} ");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var existingLogin = await _userManager.FindByNameAsync(model.Login);
                if (existingLogin != null)
                {
                    ModelState.AddModelError(string.Empty, "The login is already in use.");
                }

                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "The email is already in use.");
                }

                if (ModelState.ErrorCount > 0)
                {
                    // Return the error messages as JSON response
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = errors });
                }

                var user = new User { Name = model.Login, UserName = model.Login, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Success!", Errors = new List<string> { } } );

                }

                // Handle other registration errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(new { Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var existingEmail = await _userManager.FindByEmailAsync(model.LoginOrEmail);
                var existingLogin = await _userManager.FindByNameAsync(model.LoginOrEmail);
                if(existingLogin == null && existingEmail == null)
                {
                    return BadRequest(new { Message = "", Error = "No Such Login Exists" });
                }
                if (existingEmail != null) { model.LoginOrEmail = existingEmail.UserName; }
               
                var result = await _signInManager.PasswordSignInAsync(model.LoginOrEmail, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var editPageUrl = "/News/Edit"; // Retrieve from TempData

                    if (!string.IsNullOrEmpty(editPageUrl))
                    {
                       return Ok(new { Message = "Success!", Error = "" });
                    }
                    else
                    {
                        // If no edit page URL, redirect to a default page (e.g., the homepage)
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    return BadRequest(new { Message = "", Error = "Wrong Password!" });
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new {Message = "Success!"}); 
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            return View();
        }

        [Authorize] 
        public async Task<IActionResult> UserProfile()
        {
            var usr = await _userManager.GetUserAsync(User);
            var user = _newsDbContext.Users.Include(x => x.Logo).First(x => x.UserName == usr.UserName);
            var userProfile = new UserProfile
            {
                Name = user.Name,
                Login = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LogoUrl = user.Logo == null ? "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1200px-No-Image-Placeholder.svg.png " : user.Logo.Url()
            };

            return View(userProfile);
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var usr = await _userManager.GetUserAsync(User);
            var user = _newsDbContext.Users.Include(x => x.Logo).First(x=> x.UserName == usr.UserName);
            var userProfile = new UserProfile
            {
                Name = user.Name,
                Login = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LogoUrl = user.Logo == null ? "https://upload.wikimedia.org/wikipedia/commons/thumb/6/65/No-Image-Placeholder.svg/1200px-No-Image-Placeholder.svg.png " : user.Logo.Url()
            };

            return View(userProfile);
        }
        [HttpPost("/Account/Uploads/")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var usr = await _userManager.GetUserAsync(User);
            var user = _newsDbContext.Users.Include(x => x.Logo).First(x => x.UserName == usr.UserName);
            var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var dbFile = new DbImageFile()
            {
                Filename = filename,
                RootDirectory = "Uploads",
            };
            var localFilename =
                Path.Combine(_webHostEnvironment.WebRootPath, dbFile.RootDirectory, dbFile.Filename);
            using (var localFile = System.IO.File.Open(localFilename, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(localFile);
            }
            _newsDbContext.Images.Add(dbFile);
            user.Logo = dbFile;
            await _newsDbContext.SaveChangesAsync();
            return Ok(new { url = dbFile.Url() });
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile([FromBody] UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Update user properties
                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;

                    // Update the user in the database
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return Ok(new {Message = "Success!"});
                    }
                    else
                    {
                        // Handle errors and return appropriate response
                        return BadRequest(new { Message = "Failed to update profile", Errors = result.Errors });
                    }
                }
                else
                {
                    return NotFound(new { Message = "User not found" });
                }
            }

            return BadRequest(new { Message = "Invalid model state" });
        }

    }
}
