// Bu dosya diðer MyDbModel sýnýflarýna baðlý, o yüzden namespace'i doðru olmalý
namespace SAT242516033.Models.MyDbModels;

// Bu dosya Extensions klasöründeki kodlarý kullanýyor
using SAT242516033.Models.Extensions;

public static class MyDbModel_Extension
{
    public static IDictionary<object, object> GetOrderByItems<E>(this MyDbModel<E> myDbModel) where E : class, new()
    {
        var sortByItems = new Dictionary<object, object>();

        return sortByItems;
    }
}