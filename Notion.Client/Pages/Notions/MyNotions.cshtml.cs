using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Protos;
using System.IdentityModel.Tokens.Jwt;

namespace Notion.Client.Pages.Notions
{
    public class MyNotionsModel : PageModel
    {
        private readonly UserNotionManager.UserNotionManagerClient _userNotionClient;
        public MyNotionsModel(UserNotionManager.UserNotionManagerClient userNotionClient)
        {
            _userNotionClient = userNotionClient;
        }

        public List<UserNotion> Notions { get; set; } = [];

        [BindProperty]
        public string NotionId { get; set; }

        [BindProperty]
        public string NotionText { get; set; }

        [BindProperty]
        public string NotionIsCompleted { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var token = Request.Cookies["Jwt"];
                var headers = new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };

                var response = await _userNotionClient.GetUserNotionsAsync(new GetUserNotionsRequest(), headers);

                Notions = response.Notions.Select(notion => new UserNotion
                {
                    NotionId = notion.NotionId,
                    DateCreate = notion.DateCreate,
                    IsCompleted = notion.IsCompleted,
                    Text = notion.Text
                }).ToList();
            }
            catch (Exception) 
            {
                OnPostLogout();
            }
        }

        public IActionResult OnPostEditNotion()
        {
            TempData["NotionId"] = NotionId;
            TempData["NotionText"] = NotionText;
            TempData["NotionIsCompleted"] = NotionIsCompleted;

            return RedirectToPage("/Notions/EditNotion");
        }

        public async Task<IActionResult> OnPostDeleteNotion()
        {
            try
            {
                await _userNotionClient.DeleteUserNotionAsync(new DeleteUserNotionRequest
                {
                    NotionId = NotionId,
                });

                NotionId = "";

                return RedirectToPage("/Notions/MyNotions");
            }
            catch (Exception)
            {
                return RedirectToPage("/Notions/MyNotions");
            }
        }

        public IActionResult OnPostLogout()
        {
            Response.Cookies.Delete("Jwt");
            return RedirectToPage("/Authentication/Login");
        }
    }
}