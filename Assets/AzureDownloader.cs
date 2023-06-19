using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AzureDownloader : MonoBehaviour
{
    private string storageAccount = "taylorstalesassets";
    private string containerName = "bookdata";
    private const string fileName = "ChickenAndTheFox";
    public void DownloadFile()
    {
        string url = $"https://taylorstalesassets.blob.core.windows.net/bookdata/ChickenAndTheFox/Page_1/JSONPage_1.json";

        StartCoroutine(DownloadFileRoutine(url));
    }

    private IEnumerator DownloadFileRoutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // File downloaded successfully, do something with the downloaded data
                byte[] downloadedData = webRequest.downloadHandler.data;
                // Example: Save the data to disk
                System.IO.File.WriteAllBytes($"{Application.streamingAssetsPath}", downloadedData);
            }
            else
            {
                // Error occurred while downloading the file
                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError("Network connectivity issue. Please check your internet connection.");
                }
                else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Protocol Error: {webRequest.error}");
                }
                else
                {
                    Debug.LogError($"Download Error: {webRequest.error}");
                }
            }
        }
    }
}
