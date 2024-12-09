using Grpc.Core;
using Notion.BaseModule.Interfaces;
using Notion.DataAccess.Exceptions;
using Notion.Protos;

namespace Notion.GrpcServer.Services
{
    public class UserManagerService : UserManager.UserManagerBase
    {
        private readonly ILogger<UserManagerService> _logger;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserService _userService;
        public UserManagerService(
            ILogger<UserManagerService> logger, 
            IUserService userService,
            IJwtProvider jwtProvider)
        {
            _logger = logger;
            _userService = userService;
            _jwtProvider = jwtProvider;
        }

        public async override Task<GetUserResponce> GetUser(GetUserRequest request,
            ServerCallContext context)
        {
            try
            {
                var user = await _userService.GetUserByLoginAsync(request.Login);

                var userResponce = new GetUserResponce
                {
                    UserId = user.Id,
                    Login = request.Login,
                };

                return userResponce;
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

        public async override Task<AddUserResponce> CreateUser(AddUserRequest request,
            ServerCallContext context)
        {
            try
            {
                var newUser = await _userService.CreateUserAsync(request.Login, request.Password);

                var userResponce = new AddUserResponce
                {
                    UserId = newUser.Id,
                    Login = newUser.Login,
                };

                return userResponce;
            }
            catch (UserLoginIsUnavailableException ex)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Неизвестная ошибка: " + ex.Message));
            }
        }

        public async override Task<AuthUserResponce> AuthUser(AuthUserRequest request, ServerCallContext context)
        {
            try
            {
                var user = await _userService.GetUserByLoginAsync(request.Login);

                var checkUserData = await _userService.CheckUserDataAsync(request.Login, request.Password);

                if (checkUserData)
                {
                    var authUserResponce = new AuthUserResponce
                    {
                        Token = _jwtProvider.GenerateToken(user.Id, user.Login)
                    };

                    return authUserResponce;
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Неверные данные для авторизации"));
                }
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
    }
}