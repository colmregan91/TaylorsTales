using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public TextMeshProUGUI text;

    private PageTextList newPageList;



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

        string Pagepath = Path.Combine($"{Application.streamingAssetsPath}", "Pages", "JSONPageData.json");
        string FactPath = Path.Combine($"{Application.streamingAssetsPath}", "Facts", "Facts.json");
        StartCoroutine(DownloadPagesFromPath(Pagepath));
        StartCoroutine(DownloadFactsFromPath(FactPath));

        BookManager.OnPageChanged += CheckBundleLoaded;
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
            FactContents contents = new FactContents(curfact.FactInfo, Path.Combine($"{Application.streamingAssetsPath}", "Facts", curfact.imagesBundle + ".unity3d"));
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
            text.text += "found Fact json file \n";
            string dataAsJson = www.downloadHandler.text;
            InitFacts(dataAsJson);
        }
        else
        {
            text.text += "cannot find fact json file \n";
        }
    }

    private IEnumerator DownloadPagesFromPath(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            text.text += "found json file \n";
            string dataAsJson = www.downloadHandler.text;

            newPageList = JsonUtility.FromJson<PageTextList>(dataAsJson);

            StartCoroutine(LoadPageTextsForward(newPageList.pageTexts, BookManager.LastSavedPage == 0 ? 0 : BookManager.LastSavedPage - 1, newPageList.pageTexts.Count));

            if (BookManager.LastSavedPage >= 1)
            {
                StartCoroutine(LoadPageTextsBack(newPageList, BookManager.LastSavedPage == 0 ? 0 : BookManager.LastSavedPage - 1));

            }

            //UnityWebRequest www = UnityWebRequest.Get(path);

            //yield return www.SendWebRequest();

            //if (www.result == UnityWebRequest.Result.Success)
            //{
            //    dataAsJson = www.downloadHandler.text;
            //    PageTextList newPageList = JsonUtility.FromJson<PageTextList>(dataAsJson);

            //    // StartCoroutine(LoadIndividualPageTexts(newPageList, 0)); // load first page, make it load first 3
            //    StartCoroutine(LoadPageTexts(newPageList, 0, newPageList.pageTexts.Count));
            //    //if (BookManager.LastSavedPage == 0)
            //    //{
            //    //    StartCoroutine(LoadPageTexts(newPageList, 1, newPageList.pageTexts.Count));
            //    //    yield break;
            //    //}

            //    //if (BookManager.LastSavedPage != 1)
            //    //{
            //    //    StartCoroutine(LoadIndividualPageTexts(newPageList, BookManager.LastSavedPage - 1));// load last saved page
            //    //    StartCoroutine(LoadPageTexts(newPageList, BookManager.LastSavedPage - 2, 0));
            //    //}
            //    //StartCoroutine(LoadPageTexts(newPageList, BookManager.LastSavedPage, newPageList.pageTexts.Count));
            //}
            //else
            //{
            //    // set retry system
            //    FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;

            //    yield break; // Exit the coroutine if file loading fails
            //}
        }
    }


    //private IEnumerator LoadIndividualPageTexts(PageTextList jsonPages, int page)
    //{
    //    PageContents newContents = new PageContents();
    //    Page pageTemp = jsonPages.pageTexts[page];
    //    newContents.Texts = pageTemp.Texts;
    //    string Environmentpath = $"{Application.streamingAssetsPath}/Pages/Page_{pageTemp.pageNumber}_EnvironmentCanvas.unity3d";
    //    if (File.Exists(Environmentpath))
    //    {
    //        yield return loadPageCanvasses(Environmentpath, pageTemp.pageNumber, newContents);
    //    }
    //    else
    //    {
    //        Debug.LogError("no bundle for page " + pageTemp.pageNumber);
    //    }
    //}

    private IEnumerator LoadPageTextsBack(PageTextList jsonPages, int fromPage)
    {
        int lastTwo = fromPage - 2;
        for (int i = fromPage - 1; i >= lastTwo; i--)
        {
            if (i == -1) break;
            PageContents newContents = new PageContents();
            Page pageTemp = jsonPages.pageTexts[i];
            BookManager.AddNewPage(pageTemp.pageNumber, newContents);
            newContents.Texts = pageTemp.Texts;

            string Environmentpath = Path.Combine(Application.streamingAssetsPath, "Pages", $"Page_{ pageTemp.pageNumber}_EnvironmentCanvas.unity3d");
            text.text += "attempting page " + pageTemp.pageNumber + "\n";
            yield return loadPageCanvasses(Environmentpath, pageTemp.pageNumber, newContents);

        }
    }

    private void CheckFirstThreePages()
    {
        Debug.Log("ce");
        MainMenuCanvas.OnNewStoryClicked -= CheckFirstThreePages;
        StartCoroutine(LoadPageTextsForward(newPageList.pageTexts, BookManager.LastSavedPage == 0 ? 0 : BookManager.LastSavedPage - 1, newPageList.pageTexts.Count));
    }

    private void CheckBundleLoaded(int number, PageContents contents)
    {


        if (!BookManager.Pages.ContainsKey(number + 2) && (number + 2) <= newPageList.pageTexts.Count)
        {
            LoadNextBundleIE(newPageList.pageTexts[number + 2]);
        }
        Debug.Log("cur " + number + " checking " + (number - 2));
        if (!BookManager.Pages.ContainsKey(number - 2) && (number - 2) >= 1)
        {
            LoadNextBundleIE(newPageList.pageTexts[number - 2]);
        }
    }

    private void LoadNextBundleIE(Page page)
    {
        Debug.Log(page.pageNumber + " got it");
        StartCoroutine(LoadNextBundle(page));
    }
    private IEnumerator LoadNextBundle(Page page)
    {

        PageContents newContents = new PageContents();
        newContents.Texts = page.Texts;
        BookManager.AddNewPage(page.pageNumber - 1, newContents);
        string Environmentpath = Path.Combine(Application.streamingAssetsPath, "Pages", $"Page_{ page.pageNumber - 1}_EnvironmentCanvas.unity3d");
        text.text += "attempting page " + (page.pageNumber - 1) + "\n";
        yield return loadPageCanvasses(Environmentpath, page.pageNumber - 1, newContents);
    }

    private IEnumerator LoadPageTextsForward(List<Page> Pages, int fromPage, int bookLength)
    {
        int nextTwo = fromPage + 3;
        for (int i = fromPage; i < nextTwo; i++)
        {
            if (BookManager.Pages.ContainsKey(Pages[i].pageNumber))
            {
                continue;
            }

            if (i >= bookLength) break;

            PageContents newContents = new PageContents();
            Page pageTemp = Pages[i];
            BookManager.AddNewPage(pageTemp.pageNumber, newContents);
            newContents.Texts = pageTemp.Texts;
            Debug.Log(pageTemp.pageNumber);
            string Environmentpath = Path.Combine(Application.streamingAssetsPath, "Pages", $"Page_{ pageTemp.pageNumber}_EnvironmentCanvas.unity3d");
            text.text += "attempting page " + pageTemp.pageNumber + "\n";
            yield return loadPageCanvasses(Environmentpath, pageTemp.pageNumber, newContents);


        }
    }
    private IEnumerator loadPageCanvasses(string path, int pageNumber, PageContents newPageContents)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            // Asset Bundle is loaded successfully
            var EnvbundleReq = AssetBundle.LoadFromMemoryAsync(www.downloadHandler.data);

            yield return EnvbundleReq.isDone;
            text.text += pageNumber + " success \n";
            if (EnvbundleReq != null)
            {
                text.text += pageNumber + " got bundle \n";
                var Envbundle = EnvbundleReq.assetBundle;
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
                Envbundle.UnloadAsync(false);
            }
            else
            {
                text.text += "null bundle" + pageNumber + "\n";
            }
        }
        else
        {
            text.text += "unsuccess request" + pageNumber + "\n";
            Debug.LogWarning("no environment canvas or skyBox for page " + (pageNumber));
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

    private void OnDisable()
    {
        BookManager.OnPageChanged -= CheckBundleLoaded;
        MainMenuCanvas.OnNewStoryClicked -= CheckFirstThreePages;
        StopAllCoroutines();

    }

}
