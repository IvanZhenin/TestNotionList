using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notion.Protos;
using static System.Net.Mime.MediaTypeNames;

namespace Notion.Client.Pages.Notions
{
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
        public IActionResult OnGet()
        {
            var token = Request.Cookies["Jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Authentication/Login");

            NotionId = TempData["NotionId"]?.ToString() ?? string.Empty;
            NotionText = TempData["NotionText"] as string ?? string.Empty;
            NotionIsCompleted = TempData["NotionIsCompleted"]?.ToString()?.ToLower() != "false";

            return Page();
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