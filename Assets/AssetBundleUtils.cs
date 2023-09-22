using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AssetBundleUtils : MonoBehaviour
{
    private Queue<AssetBundle> assetBundleQueue = new Queue<AssetBundle>();
    private CancellationTokenSource cancellationTokenSource;

    // Add loaded asset bundles to the queue
    public void AddToUnloadQueue(AssetBundle bundle)
    {
        assetBundleQueue.Enqueue(bundle);
    }

    // Start unloading asset bundles from the queue
    public void StartUnloading()
    {
        cancellationTokenSource = new CancellationTokenSource();
        StartCoroutine(UnloadAssetBundles(cancellationTokenSource.Token));
    }

    // Cancel the unloading process
    public void CancelUnloading()
    {
        cancellationTokenSource.Cancel();
    }

    private IEnumerator UnloadAssetBundles(CancellationToken cancellationToken)
    {
        while (assetBundleQueue.Count > 0)
        {
            AssetBundle bundleToUnload = assetBundleQueue.Dequeue();

            if (bundleToUnload != null)
            {
                // Unload the asset bundle asynchronously
                var unloadOperation = bundleToUnload.UnloadAsync(false);

                // Wait until the unloading operation is complete or cancellation is requested
                while (!unloadOperation.isDone)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Handle cancellation here, if needed.
                        Debug.Log("Asset bundle unloading canceled.");
                        yield break;
                    }
                    yield return null;
                }

                // Optionally, handle the result of unloading here.
                Debug.Log("Asset bundle unloaded successfully.");
            }
        }
    }

    private void OnDisable()
    {
        CancelUnloading();
    }
}
