using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downloader : MonoBehaviour
{
    [SerializeField] private GameObject Canvasses;
    private string DataPath;
    private void Awake()
    {
        DataPath = $"{Application.persistentDataPath}";
        Debug.Log(DataPath);
    }
}
