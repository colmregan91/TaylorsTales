using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    string DataPath; string bundlePath;
    public string bun;
    // Start is called before the first frame update

    public void setbun(string g)
    {
        bun = g;
        Debug.Log(bun);
    }

    public void doit()
    {
        Debug.Log(bun);
        string text = File.ReadAllText(bun);
        Debug.Log(text);
        BundleJson newPage = JsonUtility.FromJson<BundleJson>(text);
        var Envbundle = newPage.bundle;
        var Envprefab = Envbundle.LoadAsset<GameObject>(Envbundle.GetAllAssetNames()[0]);
        Instantiate(Envprefab);
    }
}


public class BundleJson
{
    public AssetBundle bundle;
}
