using System.Globalization;
using System.Text.RegularExpressions;

namespace SAT242516033.Logging;

public class LogService(string filePath)
{
    // Dosyadan Logları Oku
    public async Task<List<LogEntry>> GetFileLogsAsync()
    {
        var logs = new List<LogEntry>();
        if (!File.Exists(filePath)) return logs;

        var lines = await File.ReadAllLinesAsync(filePath);
        var regex = new Regex(@"^(?<date>[\d\-T:\.Z ]+) \[(?<level>\w+)\] (?<category>[^:]+): (?<message>.*)$");

        foreach (var line in lines)
        {
            var match = regex.Match(line);
            if (match.Success)
            {
                logs.Add(new LogEntry
                {
                    Timestamp = DateTime.TryParse(match.Groups["date"].Value, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var dt) ? dt : DateTime.UtcNow,
                    Level = match.Groups["level"].Value,
                    Category = match.Groups["category"].Value,
                    Message = match.Groups["message"].Value,
                    Source = "File"
                });
            }
        }
        logs.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
        return logs;
    }

    // Veritabanı logları bu projede kullanılmıyor.
}
