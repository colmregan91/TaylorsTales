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

    public bool DownloadFromServer;

    [SerializeField] private Camera cam;
    private string GetInitialPath()
    {
        return DownloadFromServer ? "https://taylorstalesassets.blob.core.windows.net/bookdata/ChickenAndTheFox" :
            $"{Application.persistentDataPath}/../TaylorsTalesAssets/ChickenAndTheFox";
    }

    private void Start()
    {
        AssetBundle.UnloadAllAssetBundles(true);

        StartCoroutine(loadFactsAndroid());

        StartCoroutine(loadPages());

    }

    private IEnumerator loadFactsAndroid()
    {
        string factPath = Path.Combine($"{GetInitialPath()}", "Facts", "Facts.json");


        UnityWebRequest www = UnityWebRequest.Get(factPath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string dataAsJson = www.downloadHandler.text;
            InitFacts(dataAsJson);

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



    private IEnumerator loadPages()
    {

        string pageRoot = Path.Combine($"{GetInitialPath()}", "Pages", "JSONPageData.json");
        string dataAsJson;

        UnityWebRequest www = UnityWebRequest.Get(pageRoot);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            dataAsJson = www.downloadHandler.text;
            PageTextList newPageList = JsonUtility.FromJson<PageTextList>(dataAsJson);

            StartCoroutine(LoadIndividualPageTexts(newPageList, 0)); // load first page, make it load first 3

            if (BookManager.LastSavedPage == 0)
            {
                StartCoroutine(LoadPageTexts(newPageList, 1, newPageList.pageTexts.Count));
                yield break; 
            }

            if (BookManager.LastSavedPage != 1)
            {
                StartCoroutine(LoadIndividualPageTexts(newPageList, BookManager.LastSavedPage - 1));// load last saved page
                StartCoroutine(LoadPageTexts(newPageList, BookManager.LastSavedPage - 2, 0));
            }
            StartCoroutine(LoadPageTexts(newPageList, BookManager.LastSavedPage, newPageList.pageTexts.Count));
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
        yield return loadPageCanvasses(pageTemp.pageNumber, newContents);
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
                yield return loadPageCanvasses(pageTemp.pageNumber, newContents);
            }
        }
        else
        {
            for (int i = fromPage; i > toPage; i--)
            {
                PageContents newContents = new PageContents();
                Page pageTemp = jsonPages.pageTexts[i];
                newContents.Texts = pageTemp.Texts;
                yield return loadPageCanvasses(pageTemp.pageNumber, newContents);
            }
        }


    }
    private IEnumerator loadPageCanvasses(int pageNumber, PageContents newPageContents)
    {
        string pageRoot = Path.Combine($"{GetInitialPath()}", "Pages");
        string Environmentpath = $"{pageRoot}/Page_{pageNumber}_EnvironmentCanvas.unity3d";

        UnityWebRequest Envrequest = UnityWebRequestAssetBundle.GetAssetBundle(Environmentpath);

        yield return Envrequest.SendWebRequest();
        if (Envrequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle Envbundle = DownloadHandlerAssetBundle.GetContent(Envrequest);

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
