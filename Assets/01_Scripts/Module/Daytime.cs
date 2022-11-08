using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daytime : MonoBehaviour
{
    private bool isNight = false;

    [SerializeField] private float nightFogDensity;
    private float dayFogDensity;
    [SerializeField]private float fogDensityCalc;
    private float currentFogDensity;

    private void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }
    private void Update()
    {
        transform.Rotate(new Vector3(5 * Time.deltaTime, 0, 0));

        if (transform.eulerAngles.x >= 170)
            isNight = true;
        else if (transform.eulerAngles.x <= 10)
            isNight = false;

        if (isNight)
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
