using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Protos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Notion.Client.Pages.Authentication
{
    public class RegistrationModel : PageModel
    {
        private readonly UserManager.UserManagerClient _userClient;
        public RegistrationModel(UserManager.UserManagerClient userClient)
        {
            _userClient = userClient;
        }

        [BindProperty]
        public string UserLogin { get; set; }

        [BindProperty]
        public string UserPassword { get; set; }

        [BindProperty]
        public string ConfirmUserPassword { get; set; }

        public IActionResult OnGet()
        {
            var jwtToken = Request.Cookies["Jwt"];
            if (!string.IsNullOrEmpty(jwtToken))
                return RedirectToPage("/Notions/MyNotions");

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            if (ConfirmUserPassword != UserPassword)
                return Page();

            try
            {
                _userClient.CreateUser(new AddUserRequest
                {
                    Login = UserLogin,
                    Password = UserPassword
                });

                return RedirectToPage("/Authentication/Login");
            }
            catch (Exception)
            {
                return Page();
            }
        }
    }
}