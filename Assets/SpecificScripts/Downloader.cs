using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour // MAKE ASYNC AND UNLOAD ASSET BUNDLES,create asset bundle utils class
{
    private string DataPath;
    public static Action<int> OnPageDownloaded;
    private GameObject EnvironmentCanvasTemp;
    private GameObject InteractionCavnasTemp;
    private PageContents CurrentPageTemp;
    [SerializeField] private Transform canvasHolder;

    private const string BOOKNAME = "ChickenAndTheFox";
    private const string BOOKASSETFOLDER = "TaylorsTalesAssets";

    private string localPageJSONRoot;
    private string localPageRoot;

    public bool DownloadFromServer;
    public bool SERVERNEEDSUPDATE;
    public bool NEWBUILDNEEDED;
    public TextMeshProUGUI text;
    [SerializeField] private Camera cam;
    private string GetInitialPath()
    {
        return "https://taylorstalesassets.blob.core.windows.net/bookdata/ChickenAndTheFox";

    }

    private void Start()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        //localPageJSONRoot = $"{Application.persistentDataPath}/Pages/JSONPageData.json";
        ////     StartCoroutine(loadFactsAndroid());
        //localPageRoot = $"{Application.persistentDataPath}/Pages/";
        //if (File.Exists(localPageJSONRoot))
        //{
        //    Debug.Log("loading locally");
        //    StartCoroutine(loadPages());
        //}
        //else
        //{
        //    Debug.Log("from server");
        //    StartCoroutine(loadPageBundles());

        //}

        localPageJSONRoot = $"{Application.persistentDataPath}/Pages/JSONPageData.json";
        if (File.Exists(localPageJSONRoot))
        {
            StartCoroutine(DownloadFromPath(localPageJSONRoot, false));

        }
        else
        {
            StartCoroutine(DownloadFromPath(Path.Combine($"{GetInitialPath()}", "Pages", "JSONPageData.json"), true));
        }
    }

    private IEnumerator loadFactsAndroid()
    {
        string factPath = Path.Combine($"{GetInitialPath()}", "Facts", "Facts.json");


        UnityWebRequest www = UnityWebRequest.Get(factPath);
        www.downloadHandler = new DownloadHandlerFile($"{Application.persistentDataPath}/Facts/Facts.json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            //string dataAsJson = www.downloadHandler.text;
            //InitFacts(dataAsJson);

        }
        else
        {
            FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;
            yield break; // Exit the coroutine if file loading fails
        }



    }

    private void InitFacts(string dataAsJson)
    {
        FactList factsList = JsonUtility.FromJson<FactList>(dataAsJson);

        for (int i = 0; i < factsList.Facts.Count; i++)
        {
            Fact curfact = factsList.Facts[i];
            TriggerWords triggers = new TriggerWords(curfact.TriggerWords);

            FactContents contents = new FactContents(curfact.FactInfo, Path.Combine($"{GetInitialPath()}", "Facts", curfact.imagesBundle + ".unity3d"));
            FactManager.AddToFactList(triggers, contents);

        }
    }
    //private IEnumerator loadPagesFromServer()
    //{
    //    string pageRoot = Path.Combine($"{GetInitialPath()}", "Pages", "JSONPageData.json");
    //    string dataAsJson;

    //    UnityWebRequest www = UnityWebRequest.Get(pageRoot);
    //    www.downloadHandler = new DownloadHandlerFile(localPageJSONRoot);
    //    yield return www.SendWebRequest();

    //    if (www.result == UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log("succ");
    //        StartCoroutine(loadPageBundles());

    //    }
    //}

    // private IEnumerator loadPageBundles()
    // {
    //     for (int i = 1; i < 25; i++)
    //     {
    //         string Environmentpath = $"{GetInitialPath()}/Page_{i}_EnvironmentCanvas.unity3d";

    //         UnityWebRequest Envrequest = UnityWebRequestAssetBundle.GetAssetBundle(Environmentpath);
    //      //   Envrequest.downloadHandler = new DownloadHandlerFile($"{localPageRoot}/Page_{i}_EnvironmentCanvas");
    //         yield return Envrequest.SendWebRequest();

    //         if (Envrequest.result == UnityWebRequest.Result.Success)
    //         {
    //             Debug.Log("suc " + i);
    //         }
    //         else
    //         {
    //             Debug.Log(Envrequest.result);
    //         }
    //     }

    ////     StartCoroutine(loadPages());
    // }

    private IEnumerator DownloadFromPath(string path, bool fromServer)
    {
        string dataAsJson;
        UnityWebRequest www = UnityWebRequest.Get(path);

        if (fromServer)
        {
            text.text += "json server \n";
            Debug.Log("server");
            string localPath = $"{Application.persistentDataPath}/Pages/JSONPageData.json";
            www.downloadHandler = new DownloadHandlerFile(localPath);
        }
        else
        {
            text.text += "json local \n";
            Debug.Log("local");
        }


        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            dataAsJson = www.downloadHandler.text;
            PageTextList newPageList = JsonUtility.FromJson<PageTextList>(dataAsJson);

            // StartCoroutine(LoadIndividualPageTexts(newPageList, 0)); // load first page, make it load first 3
            StartCoroutine(LoadPageTexts(newPageList, 0, newPageList.pageTexts.Count));
            //if (BookManager.LastSavedPage == 0)
            //{
            //    StartCoroutine(LoadPageTexts(newPageList, 1, newPageList.pageTexts.Count));
            //    yield break;
            //}

            //if (BookManager.LastSavedPage != 1)
            //{
            //    StartCoroutine(LoadIndividualPageTexts(newPageList, BookManager.LastSavedPage - 1));// load last saved page
            //    StartCoroutine(LoadPageTexts(newPageList, BookManager.LastSavedPage - 2, 0));
            //}
            //StartCoroutine(LoadPageTexts(newPageList, BookManager.LastSavedPage, newPageList.pageTexts.Count));
        }
        else
        {
            // set retry system
            FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;

            yield break; // Exit the coroutine if file loading fails
        }
    }


    private IEnumerator LoadIndividualPageTexts(PageTextList jsonPages, int page)
    {
        PageContents newContents = new PageContents();
        Page pageTemp = jsonPages.pageTexts[page];
        newContents.Texts = pageTemp.Texts;
        string Environmentpath = $"{Application.persistentDataPath}/Pages/Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d";
        if (File.Exists(Environmentpath))
        {
            yield return loadPageCanvasses(Environmentpath, false, pageTemp.pageNumber, newContents);
        }
        else
        {
            string serverPath = Path.Combine($"{GetInitialPath()}", "Pages", $"Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d");
            yield return loadPageCanvasses(serverPath, true, pageTemp.pageNumber, newContents);
        }
    }

    private IEnumerator LoadPageTexts(PageTextList jsonPages, int fromPage, int toPage)
    {
        if (toPage > fromPage)
        {
            for (int i = fromPage; i < toPage; i++)
            {
                PageContents newContents = new PageContents();
                Page pageTemp = jsonPages.pageTexts[i];
                newContents.Texts = pageTemp.Texts;

                string Environmentpath = $"{Application.persistentDataPath}/Pages/Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d";
                FileInfo fileInfo = new FileInfo(Environmentpath);
                if (File.Exists(Environmentpath) && fileInfo.Length > 0)
                {
                    yield return loadPageCanvasses(Environmentpath, false, pageTemp.pageNumber, newContents);
                }
                else
                {
                    string serverPath = Path.Combine($"{GetInitialPath()}", "Pages", $"Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d");
                    yield return loadPageCanvasses(serverPath, true, pageTemp.pageNumber, newContents);
                }

            }
        }
        else
        {
            for (int i = fromPage; i > toPage; i--)
            {
                PageContents newContents = new PageContents();
                Page pageTemp = jsonPages.pageTexts[i];
                newContents.Texts = pageTemp.Texts;
                  string Environmentpath = $"{localPageRoot}/Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d";
                if (File.Exists(Environmentpath))
                {
                    yield return loadPageCanvasses(Environmentpath, false, pageTemp.pageNumber, newContents);
                }
                else
                {
                    string serverPath = Path.Combine($"{GetInitialPath()}", "Pages", $"Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d");
                    yield return loadPageCanvasses(serverPath, true, pageTemp.pageNumber, newContents);
                }
            }
        }


    }
    private IEnumerator loadPageCanvasses(string path, bool fromServer, int pageNumber, PageContents newPageContents)
    {


        UnityWebRequest Envrequest = UnityWebRequestAssetBundle.GetAssetBundle(path);
        if (fromServer)
        {
            text.text += $"page {pageNumber} bundle from server \n";

            Envrequest.downloadHandler = new DownloadHandlerFile($"{Application.persistentDataPath}/Pages/Page_{pageNumber}_EnvironmentCanvas.unity3d");
        }
        else
        {
            text.text += $"page {pageNumber} bundle from server \n";
        }
        yield return Envrequest.SendWebRequest();
        if (Envrequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle Envbundle = AssetBundle.LoadFromFile($"{Application.persistentDataPath}/Pages/Page_{pageNumber}_EnvironmentCanvas.unity3d");

            if (Envbundle != null)
            {

                var Envassets = Envbundle.GetAllAssetNames();

                var Envprefab = Envbundle.LoadAssetAsync<GameObject>(Envassets[1]);
                yield return Envprefab;
                var Intprefab = Envbundle.LoadAssetAsync<GameObject>(Envassets[2]);
                yield return Intprefab;
                var pageSkybox = Envbundle.LoadAssetAsync<Material>(Envbundle.GetAllAssetNames()[0]);
                yield return pageSkybox;
                newPageContents.EnvironmentCanvas = (GameObject)Envprefab.asset;
                newPageContents.SkyboxMaterial = (Material)pageSkybox.asset;
                newPageContents.InteractionCanvas = (GameObject)Intprefab.asset;
                newPageContents.InteractionCanvas.GetComponent<Canvas>().worldCamera = cam;
                //    newPageContents.interactions = newPageContents.InteractionCanvas.GetComponentsInChildren<TouchBase>();
                setUpEnvironmentCanvasses(pageNumber, newPageContents);

            }
            else
            {
                
                Debug.LogWarning("no environment canvas or skyBox for page " + (pageNumber));
                newPageContents.EnvironmentCanvas = null;
                newPageContents.SkyboxMaterial = null;
            }

            Envbundle.UnloadAsync(false);
        }
        else
        {
            text.text += $"{Envrequest.error} \n";
                     text.text += $"error \n";
            Debug.LogError("error for env canvas " + pageNumber);
        }

    }


    private void setUpEnvironmentCanvasses(int number, PageContents contents)
    {

        GameObject newPage = new GameObject($"Page_{number}");
        newPage.transform.SetParent(canvasHolder);

        EnvironmentCanvasTemp = contents.EnvironmentCanvas;

        if (EnvironmentCanvasTemp != null)
        {
            AudioSource audio = EnvironmentCanvasTemp.GetComponent<AudioSource>();
            if (audio != null)
            {
                AudioMAnager.instance.SetToBackgroundGroup(audio);
            }
            else
            {
                Debug.LogError("No background Audio for page " + number);
            }
            EnvironmentCanvasTemp.GetComponent<Canvas>().worldCamera = cam;
            Instantiate(EnvironmentCanvasTemp, newPage.transform);
        }

        newPage.SetActive(false);
        BookManager.AddNewPage(number, contents);
        BookManager.Pages[number].CanvasHolder = newPage;

        OnPageDownloaded?.Invoke(number);

    }

    private void OnDisable()
    {
        StopAllCoroutines();

    }


}
