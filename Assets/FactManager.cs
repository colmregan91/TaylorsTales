using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactManager : MonoBehaviour
{
    public static FactManager Instance;

    private Dictionary<Page, FactsAndImages> FactsAndImages = new Dictionary<Page, FactsAndImages>();

    public void AddToFactList(Page page, Fact fact, Sprite[] images)
    {
        FactsAndImages newFact = new FactsAndImages(page.Texts, images);
        FactsAndImages.Add(page, newFact);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class FactsAndImages
{
    public Sprite[] images;
    private string[] texts;

    public FactsAndImages(string[] texts, Sprite[] images)
    {
        this.texts = texts;
        this.images = images;
    }
}
