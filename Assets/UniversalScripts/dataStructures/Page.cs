using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Page
{
    public int pageNumber;
    public string[] Texts;
}

public class PageContents
{
    public GameObject CanvasHolder;
    public GameObject EnvironmentCanvas;
    public GameObject InteractionCanvas;
    public Material SkyboxMaterial;
    public string[] Texts;
}

