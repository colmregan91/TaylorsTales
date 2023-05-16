using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Downloader : MonoBehaviour
{
    [SerializeField] private GameObject Canvasses;
    private string DataPath;
    private void Awake()
    {
        DataPath = $"{Application.persistentDataPath}/../TaylorsTalesAssets/ChickenAndTheFox/";
        loadPageCanvasses();
    }

    private void loadFacts()
    {

    }

    private void loadPageCanvasses()
    {

        var files = Directory.GetDirectories(DataPath).Where(T => Path.GetFileName(T).Equals("Facts") == false).ToList();

        for (int i = 0; i < files.Count(); i++)
        {
            GameObject newPage = new GameObject($"Page_{i + 1}");
            newPage.transform.SetParent(Canvasses.transform);
            string Environmentpath = $"{files[i]}/Page_{i + 1}_EnvironmentCanvas.unity3d";
            string Interactionpath = $"{files[i]}/Page_{i + 1}_InteractionCanvas.unity3d";

            if (File.Exists(Environmentpath))
            {
                setUpEnvironmentCanvas(Environmentpath, newPage.transform);
            }
            else Debug.LogWarning("no environment canvas for page " + (i + 1));

            if (File.Exists(Interactionpath))
            {
                setUpInteractionCanvas(Interactionpath, newPage.transform);
            }
            else Debug.LogWarning("no interaction canvas for page " + (i + 1));



            //      myLoadedAssetBundle.Unload(false);

            // skybox

        }

    }


    private void setUpEnvironmentCanvas(string path, Transform parent)
    {
        var Envbundle = AssetBundle.LoadFromFile(path);
        var Envassets = Envbundle.GetAllAssetNames();
        var Envprefab = Envbundle.LoadAsset<GameObject>(Envassets[1]);
        Envprefab.GetComponent<Canvas>().worldCamera = Camera.main;
        Instantiate(Envprefab, parent);
    }

    private void setUpInteractionCanvas(string path, Transform parent)
    {
        var Intbundle = AssetBundle.LoadFromFile(path);
        var Intprefab = Intbundle.LoadAsset<GameObject>(Intbundle.GetAllAssetNames()[0]);
        Intprefab.GetComponent<Canvas>().worldCamera = Camera.main;
        Instantiate(Intprefab, parent);
    }
}
