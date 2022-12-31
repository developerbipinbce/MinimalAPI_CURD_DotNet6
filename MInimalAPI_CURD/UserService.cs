namespace MInimalAPI_CURD
{
    public class UserService : IUserService
    {
        private List<UserDto> _users => new()
        {
            new("admin", "abc123"),
        };
        public UserDto GetUser(UserRequst userRequst)
        {
            return _users.FirstOrDefault(x => string.Equals(x.UserName, userRequst.UserName) 
            && string.Equals(x.Password, userRequst.Password));
        }
    }
}
