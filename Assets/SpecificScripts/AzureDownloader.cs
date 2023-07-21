//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.Networking;

//public class AzureDownloader : MonoBehaviour
//{
//    private string storageAccount = "taylorstalesassets";
//    private string containerName = "bookdata";
//    private const string fileName = "ChickenAndTheFox";
//    private void Start()
//    {
//        string url = $"https://taylorstalesassets.blob.core.windows.net/bookdata/ChickenAndTheFox/Page_1/Page_1_EnvironmentCanvas.unity3d";

//        StartCoroutine(DownloadFileRoutine(url));
//    }

//    private IEnumerator DownloadFileRoutine(string url)
//    {

//        UnityWebRequest Envrequest = UnityWebRequestAssetBundle.GetAssetBundle(url);

//        yield return Envrequest.SendWebRequest();
//        if (Envrequest.result == UnityWebRequest.Result.Success)
//        {
//            AssetBundle Envbundle = DownloadHandlerAssetBundle.GetContent(Envrequest);
//            var Envassets = Envbundle.GetAllAssetNames();

//            var Envprefab = Envbundle.LoadAssetAsync<GameObject>(Envassets[1]);
       
//            yield return Envprefab;
//            Instantiate(Envprefab.asset);
//            var pageSkybox = Envbundle.LoadAssetAsync<Material>(Envbundle.GetAllAssetNames()[0]);
//            yield return pageSkybox;
//        }







//            yield break;
//        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
//        {
//            yield return webRequest.SendWebRequest();

//            if (webRequest.result == UnityWebRequest.Result.Success)
//            {
//                // File downloaded successfully, do something with the downloaded data
//                var downloadedData = webRequest.downloadHandler.data; 
//                // Example: Save the data to disk
//                //    var text = File.ReadAllText(downloadedData);
//                var  PageData = JsonUtility.ToJson(downloadedData);

//                string pageRoot = Path.Combine(Application.persistentDataPath, "..", "TaylorsTalesAssets");
//                string Environmentpath = $"{pageRoot}/Page_{1}/Page_{1}_testtext.json";
//                System.IO.File.WriteAllBytes(pageRoot, webRequest.downloadHandler.data); 
//            }
//            else
//            {
//                // Error occurred while downloading the file
//                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
//                {
//                    Debug.LogError("Network connectivity issue. Please check your internet connection.");
//                }
//                else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
//                {
//                    Debug.LogError($"Protocol Error: {webRequest.error}");
//                }
//                else
//                {
//                    Debug.LogError($"Download Error: {webRequest.error}");
//                }
//            }
//        }
//    }
//}
