using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AssetBundleUtils : MonoBehaviour
{
    public static AssetBundleUtils instance;
    private Queue<AssetBundle> assetBundleQueue = new Queue<AssetBundle>();

    private bool isUnloading;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToUnloadQueue(AssetBundle bundle)
    {
        assetBundleQueue.Enqueue(bundle);
    }

    // Start unloading asset bundles from the queue
    public void StartUnloading(bool loadedObjects)
    {
        if (!isUnloading)
        {
            StartCoroutine(UnloadAssetBundles(loadedObjects));
        }
   
    }

    private void OnApplicationQuit()
    {
        FindObjectOfType<LocalDownload>().CancelLoading();
        Debug.Log(assetBundleQueue.Count);
        foreach (var bundle in assetBundleQueue)
        {
            bundle.Unload(true);
        }
    }


    private IEnumerator UnloadAssetBundles( bool loadedObjects)
    {
        while (assetBundleQueue.Count > 0)
        {
            isUnloading = true;
            AssetBundle bundleToUnload = assetBundleQueue.Dequeue();

            if (bundleToUnload != null)
            {
                // Unload the asset bundle asynchronously

                var unloadOperation = bundleToUnload.UnloadAsync(loadedObjects);

                // Wait until the unloading operation is complete or cancellation is requested
                while (!unloadOperation.isDone)
                {
              
                    yield return null;
                }
                
            }
        }
        isUnloading = false;
    }
}
