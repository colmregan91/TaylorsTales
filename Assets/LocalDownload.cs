using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LocalDownload : MonoBehaviour
{
    private string localJSoNPageRoot;
    [SerializeField] private Camera cam;
    public static Action<int> OnPageDownloaded;
    private GameObject EnvironmentCanvasTemp;

    [SerializeField] private Transform canvasHolder;


    private PageTextList newPageList;
    private CancellationTokenSource cancellationTokenSource;


    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
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
        
        string Pagepath = Path.Combine($"{Application.streamingAssetsPath}", "Pages", "JSONPageData.json");
        string FactPath = Path.Combine($"{Application.streamingAssetsPath}", "Facts", "Facts.json");
        StartCoroutine(DownloadPagesFromPath(Pagepath));
        StartCoroutine(DownloadFactsFromPath(FactPath));

        ButtonCanvas.OnNextPageClicked += CheckNextBundleLoaded;
        ButtonCanvas.OnPrevPageClicked += CheckPrevBundleLoaded;
        MainMenuCanvas.OnNewStoryClicked += CheckFirstThreePages;
        //    loadFactsAndroid();
        //localJSoNPageRoot = $"{PagePath}/JSONPageData.json";
        //if (File.Exists(localJSoNPageRoot))
        //{
        //    StartCoroutine(DownloadFromPath("Assets/Resources/Pages/JSONPageData.json"));

        //}
        //else
        //{
        //    Debug.LogError("no local json file");
        //}
    }
    


    private void InitFacts(string dataAsJson)
    {
        FactList factsList = JsonUtility.FromJson<FactList>(dataAsJson);

        for (int i = 0; i < factsList.Facts.Count; i++)
        {
            Fact curfact = factsList.Facts[i];
            TriggerWords triggers = new TriggerWords(curfact.TriggerWords);
            FactContents contents = new FactContents(curfact.FactInfo,
                Path.Combine($"{Application.streamingAssetsPath}", "Facts", curfact.imagesBundle + ".unity3d"));
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

    private IEnumerator DownloadFactsFromPath(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string dataAsJson = www.downloadHandler.text;
            InitFacts(dataAsJson);
        }
    }

    private IEnumerator DownloadPagesFromPath(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string dataAsJson = www.downloadHandler.text;

            newPageList = JsonUtility.FromJson<PageTextList>(dataAsJson);
            BookManager.bookLength = newPageList.pageTexts.Count;

            StartCoroutine(LoadPageTextsForward(newPageList.pageTexts,
                BookManager.LastSavedPage == 0 ? 0 : BookManager.LastSavedPage - 1));

            if (BookManager.LastSavedPage >= 1)
            {
                StartCoroutine(LoadPageTextsBack(newPageList.pageTexts,
                    BookManager.LastSavedPage == 0 ? 0 : BookManager.LastSavedPage - 1));
            }
        }
    }

    private IEnumerator LoadPageTextsForward(List<Page> Pages, int fromPage)
    {
        int nextTwo = fromPage + 3;
        for (int i = fromPage; i < nextTwo; i++)
        {
            if (i >= BookManager.bookLength) break;
            if (i < 0) break;
            if (BookManager.Pages.ContainsKey(Pages[i].pageNumber))
            {
                continue;
            }

            PageContents newContents = new PageContents();
            Page pageTemp = Pages[i];
            newContents.Texts = pageTemp.Texts;
            BookManager.AddNewPage(pageTemp.pageNumber, newContents);

            string Environmentpath = Path.Combine(Application.streamingAssetsPath, "Pages",
                $"Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d");

            yield return loadPageCanvasses(Environmentpath, pageTemp.pageNumber, newContents);
        }

        AssetBundleUtils.instance.StartUnloading(false);
    }

    private IEnumerator LoadPageTextsBack(List<Page> Pages, int fromPage)
    {
        int prevTwo = fromPage - 3;
        int prevPage = fromPage - 1;
        for (int i = prevPage; i > prevTwo; i--)
        {
            if (i >= BookManager.bookLength) break;
            if (i < 0) break;
            if (BookManager.Pages.ContainsKey(Pages[i].pageNumber))
            {
                continue;
            }

            PageContents newContents = new PageContents();
            Page pageTemp = Pages[i];
            newContents.Texts = pageTemp.Texts;
            BookManager.AddNewPage(pageTemp.pageNumber, newContents);

            string Environmentpath = Path.Combine(Application.streamingAssetsPath, "Pages",
                $"Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d");

            yield return loadPageCanvasses(Environmentpath, pageTemp.pageNumber, newContents);
        }

        AssetBundleUtils.instance.StartUnloading(false);
    }

    private void CheckFirstThreePages()
    {
        MainMenuCanvas.OnNewStoryClicked -= CheckFirstThreePages;
        StartCoroutine(LoadPageTextsForward(newPageList.pageTexts, 0));
    }

    private void CheckPrevBundleLoaded()
    {
        int pageBeforePrevPage = BookManager.currentPageNumber - 3;

        if (!BookManager.Pages.ContainsKey(pageBeforePrevPage) && pageBeforePrevPage >= 1)
        {
            LoadBundleIE(newPageList.pageTexts[pageBeforePrevPage]);
        }
    }

    private void CheckNextBundleLoaded()
    {
        int pageAfterNext = BookManager.currentPageNumber + 2;

        if (!BookManager.Pages.ContainsKey(pageAfterNext) && (pageAfterNext) <= BookManager.bookLength)
        {
            LoadBundleIE(newPageList.pageTexts[pageAfterNext - 1]);
        }
    }

    private void LoadBundleIE(Page page)
    {
        if (!BookManager.Pages.ContainsKey(page.pageNumber))
        {
            StartCoroutine(LoadNextBundle(page));
        }
    }

    private IEnumerator LoadNextBundle(Page page)
    {
        PageContents newContents = new PageContents();
        newContents.Texts = page.Texts;
        BookManager.AddNewPage(page.pageNumber, newContents);

        string Environmentpath = Path.Combine(Application.streamingAssetsPath, "Pages",
            $"Page_{page.pageNumber}_EnvironmentCanvas.unity3d");

        yield return loadPageCanvasses(Environmentpath, page.pageNumber, newContents);
        AssetBundleUtils.instance.StartUnloading(false);
    }


    private IEnumerator loadPageCanvasses(string path, int pageNumber, PageContents newPageContents)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(("FOUND : " + path));
            // Asset Bundle is loaded successfully
            var EnvbundleReq = AssetBundle.LoadFromMemoryAsync(www.downloadHandler.data);

            while (!EnvbundleReq.isDone)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    Debug.Log("cancelled");
                    yield break;
                }

                yield return null;
            }


            if (EnvbundleReq != null)
            {
                var Envbundle = EnvbundleReq.assetBundle;
                var Envassets = Envbundle.GetAllAssetNames();

                var Envprefab = Envbundle.LoadAssetAsync<GameObject>(Envassets[1]);
                while (!Envprefab.isDone)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        Debug.Log("cancelled");
                        yield break;
                    }

                    yield return null;
                }

                var Intprefab = Envbundle.LoadAssetAsync<GameObject>(Envassets[2]);
                while (!Intprefab.isDone)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        Debug.Log("cancelled");
                        yield break;
                    }

                    yield return null;
                }

                var pageSkybox = Envbundle.LoadAssetAsync<Material>(Envbundle.GetAllAssetNames()[0]);
                while (!pageSkybox.isDone)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        Debug.Log("cancelled");
                        yield break;
                    }

                    yield return null;
                }
                AssetBundleUtils.instance.AddToUnloadQueue(Envbundle);

                newPageContents.EnvironmentCanvas = (GameObject)Envprefab.asset;
                newPageContents.SkyboxMaterial = (Material)pageSkybox.asset;
                newPageContents.InteractionCanvas = (GameObject)Intprefab.asset;
                newPageContents.InteractionCanvas.GetComponent<Canvas>().worldCamera = cam;
                //    newPageContents.interactions = newPageContents.InteractionCanvas.GetComponentsInChildren<TouchBase>();
                setUpEnvironmentCanvasses(pageNumber, newPageContents);
            }
        }
        else
        {
            Debug.Log(("CANT FIND : " + path));
            newPageContents.EnvironmentCanvas = null;
            newPageContents.SkyboxMaterial = null;
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

        BookManager.Pages[number].CanvasHolder = newPage;

        OnPageDownloaded?.Invoke(number);
    }

    public void CancelLoading()
    {
        cancellationTokenSource.Cancel();
    }

    private void OnDisable()
    {
        ButtonCanvas.OnNextPageClicked -= CheckNextBundleLoaded;
        ButtonCanvas.OnPrevPageClicked -= CheckPrevBundleLoaded;
        MainMenuCanvas.OnNewStoryClicked -= CheckFirstThreePages;
        StopAllCoroutines();
    }
}