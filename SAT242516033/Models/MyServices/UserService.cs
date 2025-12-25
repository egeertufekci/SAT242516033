using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserService : IUserService
{
    // ARTIK IdentityUser YOK, ApplicationUser VAR!
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserViewModel>> GetUsersAsync()
    {
        // Kullanıcıları çekiyoruz
        var users = await _userManager.Users.Select(u => new UserViewModel
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email
            // Eğer ApplicationUser içinde Ad, Soyad gibi özel alanların varsa onları da buraya ekleyebilirsin:
            // AdSoyad = u.Ad + " " + u.Soyad 
        }).ToListAsync();

        return users;
    }
}