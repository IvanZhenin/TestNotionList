using Grpc.Core;
using Notion.BaseModule.Interfaces;
using Notion.Protos;

namespace Notion.GrpcServer.Services
{
    public class UserNotionManagerService : UserNotionManager.UserNotionManagerBase
    {
        private readonly ILogger<UserNotionManagerService> _logger;
        private readonly INotionService _notionService;
        public UserNotionManagerService(ILogger<UserNotionManagerService> logger,
            INotionService notionService)
        {
            _logger = logger;
            _notionService = notionService;
        }

        public override async Task<GetUserNotionsResponce> GetUserNotions(GetUserNotionsRequest request,
            ServerCallContext context)
        {
            var notions = await _notionService.GetNotionListByUserIdAsync(request.UserId);

            var notionResponce = new GetUserNotionsResponce
            {
                Notions =
                {
                    notions.Select(n => new UserNotion
                    {
                        NotionId = n.Id.ToString(),
                        Text = n.Text,
                        IsCompleted = n.IsComplited,
                        DateCreate = n.DateCreate.ToString(),
                    })
                }
            };

            return notionResponce;
        }

        public override async Task<AddUserNotionResponce> AddUserNotion(AddUserNotionRequest request,
            ServerCallContext context)
        {
            var newNotion = await _notionService.AddNotionAsync(request.UserId, request.Text);

            var notionResponce = new AddUserNotionResponce
            {
                NotionId = newNotion.Id.ToString(),
                Text = newNotion.Text,
                IsCompleted = newNotion.IsComplited,
                DateCreate = newNotion.DateCreate.ToString(),
            };

            return notionResponce;
        }

        public override async Task<DeleteUserNotionResponce> DeleteUserNotion(DeleteUserNotionRequest request, 
            ServerCallContext context)
        {
            await _notionService.DeleteNotionAsync(Guid.Parse(request.NotionId));

            return new DeleteUserNotionResponce();
        }

        public override async Task<ChangeUserNotionResponce> ChangeUserNotion(ChangeUserNotionRequest request, 
            ServerCallContext context)
        {
            var changedNotion = await _notionService.ChangeNotionAsync(
                Guid.Parse(request.NotionId),
                request.Text ?? null,
                request.IsCompleted ?? null);

            var notionResponce = new ChangeUserNotionResponce
            {
                NotionId = changedNotion.Id.ToString(),
                Text = changedNotion.Text,
                IsCompleted =changedNotion.IsComplited,
                DateCreate = changedNotion.DateCreate.ToString(),
            };

            return notionResponce;
        }
    }
}