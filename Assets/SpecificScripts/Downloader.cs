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

    private GameObject EnvironmentCanvasTemp;
    private GameObject InteractionCavnasTemp;
    private PageContents CurrentPageTemp;
    [SerializeField] private Transform canvasHolder;

    private const string BOOKNAME = "ChickenAndTheFox";
    private const string BOOKASSETFOLDER = "TaylorsTalesAssets";

    private void Start()
    {
        StartCoroutine(loadFactsAndroid());
        
        StartCoroutine(loadPages());

    }

    private IEnumerator loadFactsAndroid()
    {
        string factPath = Path.Combine(Application.persistentDataPath, "..", BOOKASSETFOLDER, BOOKNAME, "Facts", "Facts.json");


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

            FactContents contents = new FactContents(curfact.FactInfo, Path.Combine(Application.persistentDataPath, "..", BOOKASSETFOLDER, BOOKNAME, "Facts", curfact.imagesBundle + ".unity3d"));
            FactManager.AddToFactList(triggers, contents);

        }
    }



    private IEnumerator loadPages()
    {
        string pageRoot = Path.Combine(Application.persistentDataPath, "..", BOOKASSETFOLDER, BOOKNAME, "Pages", "JSONPageData.json");
        string dataAsJson;

        UnityWebRequest www = UnityWebRequest.Get(pageRoot);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            dataAsJson = www.downloadHandler.text;
            StartCoroutine(LoadPageTexts(dataAsJson));
        }
        else
        {
            // set retry system
            FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;

            yield break; // Exit the coroutine if file loading fails
        }


    }

    private IEnumerator LoadPageTexts(string jsonPages)
    {
        PageTextList newPageList = JsonUtility.FromJson<PageTextList>(jsonPages);

        for (int i = 0; i < newPageList.pageTexts.Count; i++)
        {
            PageContents newContents = new PageContents();
            Page pageTemp = newPageList.pageTexts[i];
            newContents.Texts = pageTemp.Texts;
            yield return loadPageCanvasses(pageTemp.pageNumber, newContents);
        }
    }
    private IEnumerator loadPageCanvasses(int pageNumber, PageContents newPageContents)
    {
        string pageRoot = Path.Combine(Application.persistentDataPath, "..", "TaylorsTalesAssets", "ChickenAndTheFox/Pages");
        string Environmentpath = $"{pageRoot}/Page_{pageNumber}_EnvironmentCanvas.unity3d";
        string Interactionpath = $"{pageRoot}/Page_{pageNumber}_InteractionCanvas.unity3d";


        UnityWebRequest Intrequest = UnityWebRequestAssetBundle.GetAssetBundle(Interactionpath);

        yield return Intrequest.SendWebRequest();
        if (Intrequest.result == UnityWebRequest.Result.Success)
        {
            AssetBundle Intbundle = DownloadHandlerAssetBundle.GetContent(Intrequest);
            if (Intbundle != null)
            {
                var req = Intbundle.LoadAssetAsync<GameObject>(Intbundle.GetAllAssetNames()[0]);
                yield return req;
                newPageContents.InteractionCanvas = (GameObject)req.asset;
                newPageContents.InteractionCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            }
            else
            {
                Debug.LogWarning("no interaction canvas or skyBox for page " + (pageNumber));
                newPageContents.InteractionCanvas = null;
            }

            yield return Intbundle.UnloadAsync(false);
        }
        else
        {
            Debug.LogError("error for int canvas " + pageNumber);
        }



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
                var pageSkybox = Envbundle.LoadAssetAsync<Material>(Envbundle.GetAllAssetNames()[0]);
                yield return pageSkybox;
                newPageContents.EnvironmentCanvas = (GameObject)Envprefab.asset;
                newPageContents.SkyboxMaterial = (Material)pageSkybox.asset;
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
            EnvironmentCanvasTemp.GetComponent<Canvas>().worldCamera = Camera.main;
            Instantiate(EnvironmentCanvasTemp, newPage.transform);
        }


        newPage.SetActive(false);
        BookManager.AddNewPage(number, contents);
        BookManager.Pages[number].CanvasHolder = newPage;

        Debug.Log("downloaded " + number);
        if (number == 1)
        {
            OnPagesReady?.Invoke(1); // soon to be player prefs last pages number
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(true);
    }


}
