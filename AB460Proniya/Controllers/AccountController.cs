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

		public AccountController(UserManager<AppUser> manager,SignInManager<AppUser> signIn)
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


            AppUser user = new AppUser
            { 
              Name = uservm.Name,
              Surname = uservm.Surname,
              UserName = uservm.UserName,
              Email = uservm.Email,

            
            };

         IdentityResult result=  await _manager.CreateAsync(user,uservm.Password);

            if(!result.Succeeded)
            {

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);

                }

                return View();
            }
            await _signIn.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index","Home");


        }
		public async Task<IActionResult> Logout()
		{
			await _signIn.SignOutAsync();

			return RedirectToAction("Index", "Home");
		}


	}
}
