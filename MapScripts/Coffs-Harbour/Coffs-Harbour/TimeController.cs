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

    public Color darkSky = new Color(0.3f, 0.3f, 0.4f, 1.000f);
    public Color lightSky = new Color(1f, 1f, 1f, 1f);
    public float skyExposureDay;
    public float skyExposureNight;

    float timeElapsed;
    float lerpDuration = 600;
    float lerpValue;

    float exposureLerpTimeElapsed;
    public float exposureLerpDuration = 1;
    float exposureLerpValue;

    public float exposureNight;
    public float exposureDay;

    float startValue = 0;
    float endValue = 1;

    private void Start()
    {
        Debug.Log("[Coffs-Harbor] Start");

        // On start, initialize the animation, and freeze playback, then run the Play function
        sunAnim.Play("SunAnimation");
        sunAnim["SunAnimation"].speed = 0f;
        Play();
        float timeElapsed = 0;

        //StartCoroutine(TimeCheckLerp());

        // Disable all lights on start
        foreach (GameObject i in nightLights)
        {
            i.SetActive(false);
        }
    }

    #region Void Methods
    // Voids for calling from buttons
    public void Pause()
    {
        TODSlider.interactable = true;
        StopCoroutine(PrimaryLerp());
        play = false;
    }
    public void Play()
    {
        TODSlider.interactable = false;
        StartCoroutine(PrimaryLerp());
        play = true;
    }
    public void Day()
    {
        lerpValue = 0.1f;
        TODSlider.value = 0.1f;
    }
    public void Night()
    {
        lerpValue = 0.45f;
        TODSlider.value = 0.45f;
    }
    #endregion

    // Update is called every frame
    void Update()
    {
        sunAnim["SunAnimation"].normalizedTime = lerpValue;

        if (play)
        {
            TODSlider.value = lerpValue;
        }
    }

    // Coroutine driving time cycle
    IEnumerator PrimaryLerp()
    {
        Debug.Log("[Coffs Harbour] Start Coroutine");

        while (timeElapsed < lerpDuration)
        {
            lerpValue = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return new WaitUntil(() => play);
        }

        lerpValue = endValue;

        // Reset lerp for looping
        if (lerpValue == endValue)
        {
            timeElapsed = 0;
            lerpValue = startValue;
        }
    }
    /*
    IEnumerator TimeCheckLerp()
    {
        if (lerpValue <= .3f)
        {
            StartCoroutine(LerpDown());

            Debug.Log("Lerpdown .3");

            yield break;
        }
        else
        {
            if (lerpValue >= .7f)
            {
                StartCoroutine(LerpDown());

                Debug.Log("LerpDown .7");

                yield break;
            }
            else
            {
                if (lerpValue < .7f)
                {
                    if (lerpValue > .3f)
                    {
                        StartCoroutine(LerpUp());

                        Debug.Log("LerpUp");

                        yield break;
                    }
                }
            }

        }

        Debug.Log("TimeCheckLerpBroken");
    }
    IEnumerator LerpUp()
    {
        while (exposureLerpTimeElapsed < exposureLerpDuration)
        {
            exposureLerpValue = Mathf.Lerp(LowExposure, HighExposure, exposureLerpTimeElapsed / exposureLerpDuration);
            exposureLerpTimeElapsed += Time.deltaTime;

            Exposure exposure;
            if (skyVolumeProfile.TryGet(out exposure))
            {
                exposure.fixedExposure.SetValue(new FloatParameter(exposureLerpValue, true));
                Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
            }

            yield return null;
        }

        // Snaps value to HighExposure value, then pauses 1 frame, then resets time elapsed to allow lerp to run again
        exposureLerpValue = HighExposure;
        yield return null;
        exposureLerpTimeElapsed = 0;
        
    }
    IEnumerator LerpDown()
    {
        while (exposureLerpTimeElapsed < exposureLerpDuration)
        {
            exposureLerpValue = Mathf.Lerp(HighExposure, LowExposure, exposureLerpTimeElapsed / exposureLerpDuration);
            exposureLerpTimeElapsed += Time.deltaTime;

            Exposure exposure;
            if (skyVolumeProfile.TryGet(out exposure))
            {
                exposure.fixedExposure.SetValue(new FloatParameter(exposureLerpValue, true));
                Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
            }

            yield return null;
        }

        // Snaps value to LowExposure value, then pauses 1 frame, then resets time elapsed to allow lerp to run again
        exposureLerpValue = LowExposure;
        yield return null;
        exposureLerpTimeElapsed = 0;
    }
    */

    #region Slider Change
    public void Slider_Changed(float Slider_Value)
    {
        // Sets the lerp value relative to the slider value
        lerpValue = Slider_Value;
        // This sets the timeElapsed value to remain in sync with the lerpValue
        timeElapsed = (lerpDuration * lerpValue);

        #region Light Control
        // This all drives the lights turning on and off at certain times of day
        // Also drives exposure values
        if (Slider_Value <= .3f)
        {
            foreach (GameObject i in nightLights)
            {
                i.SetActive(false);

                if (skyVolumeProfile.TryGet(out Exposure exposure))
                {
                    exposure.fixedExposure.SetValue(new FloatParameter(exposureDay, true));
                    //Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
                }
                if (skyVolumeProfile.TryGet(out PhysicallyBasedSky physicallyBasedSky))
                {
                    physicallyBasedSky.horizonTint = new ColorParameter(lightSky, true);
                    physicallyBasedSky.zenithTint = new ColorParameter(lightSky, true);
                    physicallyBasedSky.exposure = new FloatParameter(skyExposureDay, true);
                    //Debug.Log("[Coffs Harbour] Horizon Tint = " + physicallyBasedSky.horizonTint);
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
                        exposure.fixedExposure.SetValue(new FloatParameter(exposureDay, true));
                        //Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
                    }
                    PhysicallyBasedSky physicallyBasedSky;
                    if (skyVolumeProfile.TryGet(out physicallyBasedSky))
                    {
                        physicallyBasedSky.horizonTint = new ColorParameter(lightSky, true);
                        physicallyBasedSky.zenithTint = new ColorParameter(lightSky, true);
                        physicallyBasedSky.exposure = new FloatParameter(skyExposureDay, true);
                        //Debug.Log("[Coffs Harbour] Horizon Tint = " + physicallyBasedSky.horizonTint);
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
                        exposure.fixedExposure.SetValue(new FloatParameter(exposureNight, true));
                        //Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
                    }
                    PhysicallyBasedSky physicallyBasedSky;
                    if (skyVolumeProfile.TryGet(out physicallyBasedSky))
                    {
                        physicallyBasedSky.horizonTint = new ColorParameter(darkSky, true);
                        physicallyBasedSky.zenithTint = new ColorParameter(darkSky, true);
                        physicallyBasedSky.exposure = new FloatParameter(skyExposureNight, true);
                        //Debug.Log("[Coffs Harbour] Horizon Tint = " + physicallyBasedSky.horizonTint);
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
        Exposure exposure;
        if (skyVolumeProfile.TryGet(out exposure))
        {
            exposure.fixedExposure.SetValue(new FloatParameter(exposureNight, true));
            Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
        }
        PhysicallyBasedSky physicallyBasedSky;
        if (skyVolumeProfile.TryGet(out physicallyBasedSky))
        {
            physicallyBasedSky.horizonTint = new ColorParameter(darkSky, true);
            Debug.Log("[Coffs Harbour] Horizon Tint = " + physicallyBasedSky.horizonTint);
        }
        /*
        Exposure exposure;
        if (skyVolumeProfile.TryGet(out exposure))
        {
            StartCoroutine(LerpUp());

            log += exposure.displayName + Environment.NewLine;
            log += "Exposure Mode: " + exposure.mode + Environment.NewLine;
            log += "Fixed Exposure: " + exposure.fixedExposure + Environment.NewLine;
        }
        else
        {
            Debug.Log("No Exposure override found");
        }
        */
        //Debug.Log(log);
    }
    public void VolumeDebug()
    {
        Debug.Log("Magic Button 2 Pressed");
        Exposure exposure;
        if (skyVolumeProfile.TryGet(out exposure))
        {
            Debug.Log("[Coffs Harbour] Exposure value = " + exposure.fixedExposure);
        }
        PhysicallyBasedSky physicallyBasedSky;
        if (skyVolumeProfile.TryGet(out physicallyBasedSky))
        {
            Debug.Log("[Coffs Harbour] Horizon Tint = " + physicallyBasedSky.horizonTint);
            Debug.Log("[Coffs Harbour] Zenith Tint = " + physicallyBasedSky.zenithTint);
            Debug.Log("[Coffs Harbour] Sky Exposure = " + physicallyBasedSky.exposure);
        }
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