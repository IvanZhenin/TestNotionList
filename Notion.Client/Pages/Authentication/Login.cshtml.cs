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
            if (User.Identity?.IsAuthenticated == true)
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
                var jwtHandler = new JwtSecurityTokenHandler();
                var jsonToken = jwtHandler.ReadToken(token) as JwtSecurityToken
                    ?? throw new Exception();

                var userId = jsonToken.Payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
                var userLogin = jsonToken.Payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString() ?? throw new Exception()),
                    new Claim(ClaimTypes.Name, userLogin.ToString() ?? throw new Exception()),
                };

                Response.Cookies.Append("Jwt", token, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = true,
                    Secure = true
                });

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToPage("/Notions/MyNotions");
            }
            catch (Exception) 
            {
                return Page();
            }
        }
    }
}
