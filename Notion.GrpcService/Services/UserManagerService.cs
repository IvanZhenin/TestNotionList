using Grpc.Core;
using Notion.BaseModule.Interfaces;
using Notion.Protos;

namespace Notion.GrpcServer.Services
{
    public class UserManagerService : UserManager.UserManagerBase
    {
        private readonly ILogger<UserManagerService> _logger;
        private readonly IUserService _userService;
        public UserManagerService(ILogger<UserManagerService> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async override Task<GetUserResponce> GetUser(GetUserRequest request,
            ServerCallContext context)
        {
            var user = await _userService.GetUserByLoginAsync(request.Login);

            var userResponce = new GetUserResponce
            {
                UserId = user.Id,
                Login = request.Login,
            };

            return userResponce;
        }

        public async override Task<AddUserResponce> CreateUser(AddUserRequest request,
            ServerCallContext context)
        {
            var newUser = await _userService.CreateUserAsync(request.Login, request.Password);

            var userResponce = new AddUserResponce
            {
                UserId = newUser.Id,
                Login = newUser.Login,
            };

            return userResponce;
        }

        public override async Task<CheckUserResponce> CheckUser(CheckUserRequest request,
            ServerCallContext context)
        {
            var checkUserData = await _userService.CheckUserDataAsync(request.Login, request.Password);

            var userResponce = new CheckUserResponce { IsValid = checkUserData };

            return userResponce;
        }
    }
}