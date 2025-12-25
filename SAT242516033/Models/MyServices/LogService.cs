using Microsoft.EntityFrameworkCore;
using SAT242516033.Data;
using SAT242516033.Models.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DbLogService
{
    private readonly MyDbModel_DbContext _context;

    public DbLogService(MyDbModel_DbContext context)
    {
        _context = context;
    }

    public async Task<List<Logs_Table>> GetLogsAsync()
    {
        return await _context.Logs_Table
                             .Take(100) // Sadece son 100'ü al (Sıralama yapmadan)
                             .ToListAsync();
    }
}