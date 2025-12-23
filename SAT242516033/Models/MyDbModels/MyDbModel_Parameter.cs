namespace MyDbModels;

public interface IMyDbModel_Parameter
{
    string OrderBy { get; set; }
    int PageNumber { get; set; }
    int PageSize { get; set; }
    int TotalPageCount { get; }
    int TotalRecordCount { get; set; }
    IDictionary<string, object> Params { get; set; }
    IDictionary<string, string> Where { get; set; }
    bool HasNext { get; }
    bool HasPrevious { get; }
}

public class MyDbModel_Parameter : IMyDbModel_Parameter
{
    public static MyDbModel_Parameter Create(int pageNumber, int pageSize, string orderBy) => new(pageNumber, pageSize, orderBy);
    public static MyDbModel_Parameter Create() => new MyDbModel_Parameter(1, 10, "Id asc");

    private MyDbModel_Parameter(int pageNumber, int pageSize, string orderBy)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        OrderBy = orderBy;

        Params = new Dictionary<string, object>();
        Where = new Dictionary<string, string>();
    }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalRecordCount { get; set; }
    public int TotalPageCount => (int)Math.Ceiling(TotalRecordCount / (double)(PageSize <= 0 ? 1 : PageSize));
    public string OrderBy { get; set; } = "Id asc";
    public IDictionary<string, object> Params { get; set; }
    public IDictionary<string, string> Where { get; set; }

    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPageCount;
}
