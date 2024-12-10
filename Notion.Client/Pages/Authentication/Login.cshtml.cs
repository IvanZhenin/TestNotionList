using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Protos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Notion.Client.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly UserManager.UserManagerClient _userClient;
        public LoginModel(UserManager.UserManagerClient userClient)
        {
            _userClient = userClient;
        }

        [BindProperty]
        public string UserLogin { get; set; }

        [BindProperty]
        public string UserPassword { get; set; }

        public IActionResult OnGet()
        {
            var jwtToken = Request.Cookies["Jwt"];
            if (!string.IsNullOrEmpty(jwtToken))
                return RedirectToPage("/Notions/MyNotions");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var authRequest = await _userClient.AuthUserAsync(new AuthUserRequest
                {
                    Login = UserLogin,
                    Password = UserPassword
                });

                string token = authRequest.Token;
               
                Response.Cookies.Append("Jwt", token, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = true,
                    Secure = true
                });

                return RedirectToPage("/Notions/MyNotions");
            }
            catch (Exception) 
            {
                return Page();
            }
        }
    }
}