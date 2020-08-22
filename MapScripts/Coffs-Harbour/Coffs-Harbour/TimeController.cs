using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using System;
using UnityEngine.Rendering.HighDefinition;

public class TimeController : MonoBehaviour
{
    public Animation sunAnim;
    public Slider TODSlider;
    public VolumeProfile skyVolumeProfile;
    public GameObject[] nightLights;

    bool play;

    float timeElapsed;
    float lerpDuration = 600;
    float lerpValue;

    float startValue = 0;
    float endValue = 1;

    public object Slider_Value { get; private set; }

    private void Start()
    {
        Debug.Log("[Coffs-Harbor] Start");

        // On start, initialize the animation, and freeze playback
        sunAnim.Play("SunAnimation");
        sunAnim["SunAnimation"].speed = 0f;
        Play();
        float timeElapsed = 0;

        // Disable all lights on start
        foreach (GameObject i in nightLights)
        {
            i.SetActive(false);
        }
    }

    public void Pause()
    {
        TODSlider.interactable = true;
        StopCoroutine(Lerp());
        play = false;
    }
    public void Play()
    {
        TODSlider.interactable = false;
        StartCoroutine(Lerp());
        play = true;
    }
    public void Day()
    {
        lerpValue = 0.1f;
    }
    public void Night()
    {
        lerpValue = 0.45f;
    }

    void Update()
    {
        sunAnim["SunAnimation"].normalizedTime = lerpValue;

        if (play)
        {
            TODSlider.value = lerpValue;
        }
    }

    IEnumerator Lerp()
    {
        Debug.Log("[Coffs Harbour] Start Coroutine");

        //float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            lerpValue = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            //yield return null;
            yield return new WaitUntil(() => play);
        }

        lerpValue = endValue;

        if (lerpValue == endValue)
        {
            timeElapsed = 0;
            lerpValue = startValue;
        }
    }

    #region Slider Change
    public void Slider_Changed(float Slider_Value)
    {
        // Uncomment to debug the float of the slider value

        // Debug.Log("[Coffs-Harbor] Slider Value" + Slider_Value);

        // Sets the animation relative to the slider value
        lerpValue = Slider_Value;
        timeElapsed = (lerpDuration * lerpValue);

        #region Light Control
        // This all drives the lights turning on and off at certain times of day
        // Also drives exposure values
        if (Slider_Value <= .3f)
        {
            foreach (GameObject i in nightLights)
            {
                i.SetActive(false);

                Exposure exposure;
                if (skyVolumeProfile.TryGet(out exposure))
                {
                    exposure.fixedExposure.SetValue(new FloatParameter(10.25f, true));
                }
            }
        }
        else
        {
            if (Slider_Value >= .7f)
            {
                foreach (GameObject i in nightLights)
                {
                    i.SetActive(false);

                    Exposure exposure;
                    if (skyVolumeProfile.TryGet(out exposure))
                    {
                        exposure.fixedExposure.SetValue(new FloatParameter(10.25f, true));
                    }
                }
            }
            else
            {
                foreach (GameObject i in nightLights)
                {
                    i.SetActive(true);

                    Exposure exposure;
                    if (skyVolumeProfile.TryGet(out exposure))
                    {
                        exposure.fixedExposure.SetValue(new FloatParameter(12.5f, true));
                    }
                }
            }
        }
        #endregion
    }
    #endregion
    #region Volume Debug
    public void VolumeTest()
    {
        Debug.Log("Magic Button Pressed");

        var log = "Exposure values are: ";

        Exposure exposure;
        if (skyVolumeProfile.TryGet(out exposure))
        {
            exposure.fixedExposure.SetValue(new FloatParameter(15.0f, true));

            log += exposure.displayName + Environment.NewLine;
            log += "Exposure Mode: " + exposure.mode + Environment.NewLine;
            log += "Fixed Exposure: " + exposure.fixedExposure + Environment.NewLine;
        }
        else
        {
            Debug.Log("No Exposure override found");
        }

        Debug.Log(log);
    }
    #endregion
}

#region Volume Override List
/*
        var log = "Current volume overrides are: ";

        foreach (var component in skyVolumeProfile.components)
        {
            log += component.name + Environment.NewLine;

            foreach (var parameter in component.parameters)
            {
                log += $"--{parameter}: {parameter.GetValue<FloatParameter>()}{Environment.NewLine}";
            }
        }

        Debug.Log(log);
        */
#endregion