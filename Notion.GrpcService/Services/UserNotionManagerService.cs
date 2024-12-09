using Grpc.Core;
using Notion.BaseModule.Interfaces;
using Notion.DataAccess.Exceptions;
using Notion.Protos;
using System.Security.Claims;

namespace Notion.GrpcServer.Services
{
    public class UserNotionManagerService : UserNotionManager.UserNotionManagerBase
    {
        private readonly ILogger<UserNotionManagerService> _logger;
        private readonly IUserService _userService;
        private readonly INotionService _notionService;
        public UserNotionManagerService(ILogger<UserNotionManagerService> logger,
            INotionService notionService,
            IUserService userService)
        {
            _logger = logger;
            _notionService = notionService;
            _userService = userService;
        }

        public override async Task<GetUserNotionsResponce> GetUserNotions(GetUserNotionsRequest request,
            ServerCallContext context)
        {
            try
            {
                var userLogin = context.GetHttpContext()?.User?.FindFirst(ClaimTypes.Name)?.Value
                    ?? throw new RpcException(new Status(StatusCode.Unauthenticated, "Не авторизован"));

                var notions = await _notionService.GetNotionListByUserLoginAsync(userLogin);

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
            catch (UserByLoginNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Неизвестная ошибка: " + ex.Message));
            }
        }

        public override async Task<AddUserNotionResponce> AddUserNotion(AddUserNotionRequest request,
            ServerCallContext context)
        {
            try
            {
                var userLogin = context.GetHttpContext()?.User?.FindFirst(ClaimTypes.Name)?.Value
                    ?? throw new RpcException(new Status(StatusCode.Unauthenticated, "Не авторизован"));

                var newNotion = await _notionService.AddNotionAsync(userLogin, request.Text);

                var notionResponce = new AddUserNotionResponce
                {
                    NotionId = newNotion.Id.ToString(),
                    Text = newNotion.Text,
                    IsCompleted = newNotion.IsComplited,
                    DateCreate = newNotion.DateCreate.ToString(),
                };

                return notionResponce;
            }
            catch (UserByLoginNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Неизвестная ошибка: " + ex.Message));
            }
        }

        public override async Task<DeleteUserNotionResponce> DeleteUserNotion(DeleteUserNotionRequest request,
            ServerCallContext context)
        {
            try
            {
                await _notionService.DeleteNotionAsync(Guid.Parse(request.NotionId));
                return new DeleteUserNotionResponce();
            }
            catch (UserNotionNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Неизвестная ошибка: " + ex.Message));
            }
        }

        public override async Task<ChangeUserNotionResponce> ChangeUserNotion(ChangeUserNotionRequest request,
            ServerCallContext context)
        {
            try
            {
                var changedNotion = await _notionService.ChangeNotionAsync(
                    Guid.Parse(request.NotionId),
                    request.Text ?? null,
                    request.IsCompleted ?? null);

                var notionResponce = new ChangeUserNotionResponce
                {
                    NotionId = changedNotion.Id.ToString(),
                    Text = changedNotion.Text,
                    IsCompleted = changedNotion.IsComplited,
                    DateCreate = changedNotion.DateCreate.ToString(),
                };

                return notionResponce;
            }
            catch (UserNotionNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Неизвестная ошибка: " + ex.Message));
            }
        }
    }
}