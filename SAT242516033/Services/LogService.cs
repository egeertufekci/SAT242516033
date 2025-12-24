using System;
using System.Threading.Tasks;
using SAT242516033.Data;
using UrunSiparisTakip.Models;

namespace SAT242516033.Services;

public interface ILogService
{
    Task LogAsync(string level, string message);
}

public class LogService(ApplicationDbContext context) : ILogService
{
    public async Task LogAsync(string level, string message)
    {
        context.LogsTable.Add(new LogsTable
        {
            LogLevel = level,
            Message = message,
            CreatedAt = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }
}
