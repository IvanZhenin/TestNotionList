using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Protos;

namespace Notion.Client.Pages.Notions
{
    [Authorize]
    public class CreateNewNotionModel : PageModel
    {
        private readonly UserNotionManager.UserNotionManagerClient _userNotionClient;
        public CreateNewNotionModel(UserNotionManager.UserNotionManagerClient userNotionClient)
        {
            _userNotionClient = userNotionClient;
        }

        [BindProperty]
        public string NotionText { get; set; }

        public async Task<IActionResult> OnPostAsync() 
        {
            try
            {
                var token = Request.Cookies["Jwt"];
                var headers = new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };

                await _userNotionClient.AddUserNotionAsync(new AddUserNotionRequest
                {
                    Text = NotionText,
                }, headers);

                return RedirectToPage("/Notions/MyNotions");
            }
            catch (Exception)
            { 
                return Page();
            }
        }
    }
}