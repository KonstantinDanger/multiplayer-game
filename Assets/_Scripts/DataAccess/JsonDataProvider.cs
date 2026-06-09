using UnityEngine;

public class JsonDataProvider : IDataProvider
{
    private readonly string _dataAccessKey;

    public JsonDataProvider(string dataAccessKey)
        => _dataAccessKey = dataAccessKey;

    public T Load<T>()
    {
        string str = PlayerPrefs.GetString(_dataAccessKey);
        T data = JsonUtility.FromJson<T>(str);
        return data;
    }

    public void Save<T>(T data)
    {
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(_dataAccessKey, json);
    }
}