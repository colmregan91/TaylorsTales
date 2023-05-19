using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactManager : MonoBehaviour
{
    public static FactManager Instance;

    private Dictionary<string, Fact> FactsAndImages = new Dictionary<string, Fact>();

    public void AddToFactList( Fact fact, Sprite[] images)
    {
   
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
