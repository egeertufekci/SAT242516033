using System.Collections.Generic;
using System.Threading.Tasks;

public interface IUserService
{
    Task<List<UserViewModel>> GetUsersAsync();
}