using AB460Proniya.DAL;
using AB460Proniya.Models;
using AB460Proniya.ModelsVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AB460Proniya.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signIn;

        public AccountController(UserManager<AppUser> manager, SignInManager<AppUser> signIn)
        {
            _manager = manager;
            _signIn = signIn;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM uservm)
        {

            if (!ModelState.IsValid) return View();

            uservm.Surname = uservm.Surname.Trim();

            uservm.Name = uservm.Name.Trim();

            string surname = Char
                .ToUpper(uservm.Surname[0]) + uservm.Surname
                .Substring(1)
                .ToLower();

            string name = Char
                .ToUpper(uservm.Name[0]) + uservm.Name
                .Substring(1)
                .ToLower();



            AppUser user = new AppUser
            {
                Name = name,
                Surname = surname,
                UserName = uservm.UserName,
                Email = uservm.Email,
                Gender = uservm.Gender


            };

            IdentityResult result = await _manager.CreateAsync(user, uservm.Password);

            if (!result.Succeeded)
            {

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);

                }

                return View();
            }
            await _signIn.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");


        }



        public IActionResult Login()
        {


            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginvm, string? returnUrl)
        {

            if (!ModelState.IsValid) return View();

            AppUser user = await _manager.FindByNameAsync(loginvm.UserOrEmail);

            if(user == null)
            {

                user = await _manager.FindByEmailAsync(loginvm.UserOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError(String.Empty, "UserName, Email or Password is wrong");

                    return View();

                }
            }

          var result = await _signIn.PasswordSignInAsync(user, loginvm.Password, loginvm.IsRemember, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Həddindən artıq cəhd,daha sonra yenidən cəhd edin");
                return View();
            }

            if (!result.Succeeded)
            {

                ModelState.AddModelError(String.Empty, "UserName, Email or Password is wrong");
                return View();

            }


            if (returnUrl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnUrl);

        }




        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


    }
}
