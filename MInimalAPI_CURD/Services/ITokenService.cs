using MInimalAPI_CURD.Models;

namespace MInimalAPI_CURD.Services
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, string audience, UserDto user);
    }
}
