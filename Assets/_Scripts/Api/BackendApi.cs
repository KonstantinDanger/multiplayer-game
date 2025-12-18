using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class BackendApi
{
    private const string BackendUrl = "http://localhost:3000";

    //LogIn (post)
    //AddPlayer (Register) (post)
    //UpdatePlayerData (post)
    //FetchPlayerData (get)

    public static async void CreateMatch(GameMatchData data)
    {
        return;

        string json = JsonUtility.ToJson(data.MatchData);

        UnityEngine.Debug.Log("json " + json);
        using UnityWebRequest request = UnityWebRequest.PostWwwForm($"{BackendUrl}/api/matches", json);

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        await SendRequest(request);
    }

    public static async Task<string> FetchPlayer()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{BackendUrl}/player");
        string data = await SendRequest(request);
        return data;
    }

    private static async Task<string> SendRequest(UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result == UnityWebRequest.Result.Success)
            return request.downloadHandler.text;

        return null;
    }
}
