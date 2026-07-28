namespace Revel._808nd.com.Interfaces
{



    public interface IIntegerFromPrimaryKey<T, U>
        where T : class
        where U : class
    {
        //IEnumerable<T> List();
        T Get(U id);
    }
}
