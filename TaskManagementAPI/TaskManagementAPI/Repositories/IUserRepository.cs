using TaskManagementAPI.Models;

namespace TaskManagementAPI.Repositories
{
    public class IUserRepository
    {
        List<User> GetAllUsers();

        User GetUserById(int id);

        int AddUser(User user);
    }
}
