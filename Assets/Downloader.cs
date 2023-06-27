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

    public static Action OnFilesDownloaded;
    public static Action<int> OnPagesReady;

    private GameObject EnvironmentCanvasTemp;
    private GameObject InteractionCavnasTemp;
    private PageContents CurrentPageTemp;
    [SerializeField] private Transform canvasHolder;
    private void Start()
    {
        StartCoroutine(loadFactsAndroid());

        StartCoroutine(loadPages());
    }

    //private void loadFacts()
    //{
    //    string factPath = $"{DataPath}/Facts/Facts.json";

    //    string facts = File.ReadAllText(factPath);
    //    FactList factsList = JsonUtility.FromJson<FactList>(facts);
    //    FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = facts;
    //    for (int i = 0; i < factsList.Facts.Count; i++)
    //    {
    //        Fact curfact = factsList.Facts[i];
    //        TriggerWords triggers = new TriggerWords(curfact.TriggerWords);
    //        AssetBundle factImageBundle = AssetBundle.LoadFromFile($"{DataPath}/Facts/{curfact.imagesBundle}.unity3d");
    //        FactContents contents = new FactContents(curfact.FactInfo, factImageBundle);

    //        FactManager.AddToFactList(triggers, contents);
    //    }
    //}



    private IEnumerator loadFactsAndroid()
    {
        string factPath = Path.Combine(Application.persistentDataPath, "..", "TaylorsTalesAssets", "ChickenAndTheFox", "Facts", "Facts.json");


        UnityWebRequest www = UnityWebRequest.Get(factPath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string dataAsJson = www.downloadHandler.text;
            loadFactBundles(dataAsJson);
        }
        else
        {
            FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;
            yield break; // Exit the coroutine if file loading fails
        }



    }

    private void loadFactBundles(string dataAsJson)
    {
        FactList factsList = JsonUtility.FromJson<FactList>(dataAsJson);

        for (int i = 0; i < factsList.Facts.Count; i++)
        {
            Fact curfact = factsList.Facts[i];
            TriggerWords triggers = new TriggerWords(curfact.TriggerWords);
            AssetBundle factImageBundle = AssetBundle.LoadFromFile(Path.Combine(Application.persistentDataPath, "..", "TaylorsTalesAssets", "ChickenAndTheFox", "Facts", curfact.imagesBundle + ".unity3d"));
            FactContents contents = new FactContents(curfact.FactInfo, factImageBundle);

            FactManager.AddToFactList(triggers, contents);
        }
    }




    private IEnumerator loadPages()
    {
        string pageRoot = Path.Combine(Application.persistentDataPath, "..", "TaylorsTalesAssets", "ChickenAndTheFox/Pages");
        string dataAsJson;

        for (int i = 0; i < 30; i++)
        {
            string pagepath = $"{pageRoot}/Page_{i + 1}/JSONPage_{i + 1}.json";

            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(pagepath);
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log(pagepath);
                dataAsJson = www.downloadHandler.text;
                Page newPage = JsonUtility.FromJson<Page>(dataAsJson);
                PageContents newContents = new PageContents();
                newContents.Texts = newPage.Texts;

                loadPageCanvasses(newPage, newContents);

                BookManager.AddNewPage(newPage.pageNumber, newContents);
            }
            else
            {

                setUpEnvironmentCanvasses();
                OnFilesDownloaded?.Invoke();
                yield break; // Exit the coroutine if file loading fails
            }


        }



    }


    private void loadPageCanvasses(Page page, PageContents newPageContents)
    {
        string pageRoot = Path.Combine(Application.persistentDataPath, "..", "TaylorsTalesAssets", "ChickenAndTheFox/Pages");
        string Environmentpath = $"{pageRoot}/Page_{page.pageNumber}/Page_{page.pageNumber}_EnvironmentCanvas.unity3d";
        string Interactionpath = $"{pageRoot}/Page_{page.pageNumber}/Page_{page.pageNumber}_InteractionCanvas.unity3d";

        var Envbundle = AssetBundle.LoadFromFile(Environmentpath);

        if (Envbundle != null)
        {



            //if (page.pageNumber == 1)
            //{
            //    BundleJson json = new BundleJson();
            //    json.bundle = Envbundle;

            //    var f = JsonUtility.ToJson(json);
            //    test.setbun(f);
            //}

            var Envassets = Envbundle.GetAllAssetNames();
            var Envprefab = Envbundle.LoadAsset<GameObject>(Envassets[1]);
            var pageSkybox = Envbundle.LoadAsset<Material>(Envbundle.GetAllAssetNames()[0]);

            newPageContents.EnvironmentCanvas = Envprefab;
            newPageContents.SkyboxMaterial = pageSkybox;
        }
        else
        {
            Debug.LogWarning("no environment canvas or skyBox for page " + (page.pageNumber));
            newPageContents.EnvironmentCanvas = null;
            newPageContents.SkyboxMaterial = null;
        }

        var Intbundle = AssetBundle.LoadFromFile(Interactionpath);
        if (Intbundle != null)
        {

            for (int i = 0; i < Intbundle.GetAllAssetNames().Length; i++)
            {
                Debug.Log(Intbundle.GetAllAssetNames()[i]);
            }

            var Intprefab = Intbundle.LoadAsset<GameObject>(Intbundle.GetAllAssetNames()[0]);

            newPageContents.InteractionCanvas = Intprefab;
            newPageContents.InteractionCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        }
        else
        {
            Debug.LogWarning("no interaction canvas for page " + (page.pageNumber));
            newPageContents.InteractionCanvas = null;
        }

    }


    private void setUpEnvironmentCanvasses()
    {
        for (int i = 1; i <= BookManager.bookLength; i++)
        {
            CurrentPageTemp = BookManager.Pages[i];
            GameObject newPage = new GameObject($"Page_{i}");
            newPage.transform.SetParent(canvasHolder);

            EnvironmentCanvasTemp = CurrentPageTemp.EnvironmentCanvas;

            if (EnvironmentCanvasTemp != null)
            {
                EnvironmentCanvasTemp.GetComponent<Canvas>().worldCamera = Camera.main;
                CurrentPageTemp.EnvironmentCanvas = Instantiate(EnvironmentCanvasTemp, newPage.transform);
            }


            newPage.SetActive(false);
            BookManager.Pages[i].CanvasHolder = newPage;
        }
        Debug.Log("[pages instantiated");

        OnPagesReady?.Invoke(1); // soon to be player prefs last pages number

    }





}
