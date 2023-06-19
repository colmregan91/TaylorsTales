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
        DataPath = $"Assets/StreamingAssets 1/ChickenAndTheFox";
        loadFactsAndroid();
      //  StartCoroutine(loadFactsAndroid());
       // loadPages();

        //      setUpEnvironmentCanvasses();
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



    private void loadFactsAndroid()
    {
      //  string factPath = Path.Combine(Application.streamingAssetsPath, ""); 
        var dataAsJson = Resources.Load<TextAsset>("ChickenAndTheFox/Facts/Facts");
        string jsonContents = dataAsJson.text;

        //UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(factPath);
        //yield return www.SendWebRequest();

        //if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        //{
        //    dataAsJson = www.downloadHandler.text;
        //}
        //else
        //{
        //    FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;
        //    yield break; // Exit the coroutine if file loading fails
        //}
        FactList factsList = JsonUtility.FromJson<FactList>(jsonContents);

        for (int i = 0; i < factsList.Facts.Count; i++)
        {
            Fact curfact = factsList.Facts[i];
            TriggerWords triggers = new TriggerWords(curfact.TriggerWords);
            AssetBundle factImageBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "ChickenAndTheFox", "Facts", curfact.imagesBundle + ".unity3d"));
            FactContents contents = new FactContents(curfact.FactInfo, factImageBundle);

            FactManager.AddToFactList(triggers, contents);
        }
    }




    private IEnumerator loadPages()
    {
        string factPath = Path.Combine(Application.streamingAssetsPath, "ChickenAndTheFox/Facts/Facts.json");
        string dataAsJson;


        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(factPath);
        yield return www.SendWebRequest();

        if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            dataAsJson = www.downloadHandler.text;
        }
        else
        {
            FindObjectOfType<WordHighlighting>().gameObject.GetComponent<TextMeshProUGUI>().text = www.error;
            yield break; // Exit the coroutine if file loading fails
        }

        string path = Path.Combine(Application.streamingAssetsPath, "ChickenAndTheFox");
        var Pagefiles = Directory.GetDirectories(path).Where(T => Path.GetFileName(T).Equals("Facts") == false).ToList();
      //  UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(factPath);




        for (int i = 0; i < Pagefiles.Count(); i++)
        {

            string pagepath = $"{Pagefiles[i]}/JSONPage_{i + 1}.json";
            string text = File.ReadAllText(pagepath);
            Page newPage = JsonUtility.FromJson<Page>(text);
            PageContents newContents = new PageContents();
            newContents.Texts = newPage.Texts;
            loadPageCanvasses(newPage, newContents);

            BookManager.AddNewPage(newPage.pageNumber, newContents);
        }

        //     OnFilesDownloaded?.Invoke();
    }
    // JSONPage_1

    private void loadPageCanvasses(Page page, PageContents newPageContents)
    {

        var files = Directory.GetDirectories(DataPath).Where(T => Path.GetFileName(T).Equals("Facts") == false).ToList();

        string Environmentpath = $"{DataPath}/Page_{page.pageNumber}/Page_{page.pageNumber}_EnvironmentCanvas.unity3d";
        string Interactionpath = $"{DataPath}/Page_{page.pageNumber}/Page_{page.pageNumber}_InteractionCanvas.unity3d";

        if (File.Exists(Environmentpath))
        {
            var Envbundle = AssetBundle.LoadFromFile(Environmentpath);


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


        if (File.Exists(Interactionpath))
        {
            var Intbundle = AssetBundle.LoadFromFile(Interactionpath);
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

        //       OnPagesReady?.Invoke(1); // soon to be player prefs last pages number

    }





}
