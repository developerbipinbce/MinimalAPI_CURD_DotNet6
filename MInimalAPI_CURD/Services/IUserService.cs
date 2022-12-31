using MInimalAPI_CURD.Models;

namespace MInimalAPI_CURD.Services
{
    public interface IUserService
    {
        UserDto GetUser(UserRequst userRequst);
    }
}
