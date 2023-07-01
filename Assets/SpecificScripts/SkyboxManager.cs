using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxManager : MonoBehaviour // do not need this
{
    private const string ROT = "_Rotation";

    void Update()
    {
        RenderSettings.skybox.SetFloat(ROT, Time.time);
    }
}
