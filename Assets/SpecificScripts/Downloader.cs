using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Downloader : MonoBehaviour // MAKE ASYNC AND UNLOAD ASSET BUNDLES,create asset bundle utils class
{
    private string DataPath;

    public static Action<int> OnPagesReady;
    public static Action OnPageDownloaded;
    private GameObject EnvironmentCanvasTemp;
    private GameObject InteractionCavnasTemp;
    private PageContents CurrentPageTemp;
    [SerializeField] private Transform canvasHolder;

    private const string BOOKNAME = "ChickenAndTheFox";
    private const string BOOKASSETFOLDER = "TaylorsTalesAssets";

    private int LastSavedPage => PlayerPrefs.GetInt("Page");
    public bool DownloadFromServer;

    [SerializeField] private Camera cam;
    private string GetInitialPath()
    {
        return DownloadFromServer ? "https://taylorstalesassets.blob.core.windows.net/bookdata/ChickenAndTheFox" :
            $"{Application.persistentDataPath}/../TaylorsTalesAssets/ChickenAndTheFox";
    }

    private void Start()
    {
          // PlayerPrefs.SetInt("Page", );

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

            StartCoroutine(LoadPageTexts(newPageList, LastSavedPage, newPageList.pageTexts.Count));
            StartCoroutine(LoadPageTexts(newPageList, LastSavedPage-2, -1));
        }
        else
        {
            // set retry system
            FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;

            yield break; // Exit the coroutine if file loading fails
        }


    }

    private IEnumerator LoadPageTexts(PageTextList jsonPages, int fromPage, int toPage)
    {
        if (toPage > fromPage)
        {
            for (int i = fromPage - 1; i < toPage; i++)
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
                newPageContents.interactions = newPageContents.InteractionCanvas.GetComponentsInChildren<TouchBase>();
                setUpEnvironmentCanvasses(pageNumber, newPageContents);

            }
            else
            {
                Debug.LogWarning("no environment canvas or skyBox for page " + (pageNumber));
                newPageContents.EnvironmentCanvas = null;
                newPageContents.SkyboxMaterial = null;
            }

            yield return Envbundle.UnloadAsync(false);
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
            EnvironmentCanvasTemp.GetComponent<Canvas>().worldCamera = cam;
            Instantiate(EnvironmentCanvasTemp, newPage.transform);
        }


        newPage.SetActive(false);
        BookManager.AddNewPage(number, contents);
        BookManager.Pages[number].CanvasHolder = newPage;

        OnPageDownloaded?.Invoke();
        if (number == LastSavedPage)
        {
            OnPagesReady?.Invoke(LastSavedPage); // soon to be player prefs last pages number
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(true);
    }


}
