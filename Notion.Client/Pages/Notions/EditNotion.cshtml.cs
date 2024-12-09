using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Protos;
using static System.Net.Mime.MediaTypeNames;

namespace Notion.Client.Pages.Notions
{
    [Authorize]
    public class EditNotionModel : PageModel
    {
        private readonly UserNotionManager.UserNotionManagerClient _userNotionClient;
        public EditNotionModel(UserNotionManager.UserNotionManagerClient userNotionClient)
        {
            _userNotionClient = userNotionClient;
        }

        [BindProperty]
        public string NotionId { get; set; }

        [BindProperty]
        public string NotionText { get; set; }

        [BindProperty]
        public bool NotionIsCompleted { get; set; }

        public void OnGet()
        {
            NotionId = TempData["NotionId"]?.ToString() ?? string.Empty;
            NotionText = TempData["NotionText"] as string ?? string.Empty;
            NotionIsCompleted = TempData["NotionIsCompleted"]?.ToString()?.ToLower() != "false";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var token = Request.Cookies["Jwt"];
                var headers = new Metadata
                {
                    { "Authorization", $"Bearer {token}" }
                };

                await _userNotionClient.ChangeUserNotionAsync(new ChangeUserNotionRequest
                {
                    NotionId = NotionId,
                    Text = NotionText,
                    IsCompleted = NotionIsCompleted,
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