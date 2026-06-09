public interface IDataProvider
{
    void Save<T>(T data);
    T Load<T>();
}
