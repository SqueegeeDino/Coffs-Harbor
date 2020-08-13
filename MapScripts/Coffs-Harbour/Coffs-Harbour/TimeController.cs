using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    public Animation sunAnim;
    public Slider TODSlider;
    public GameObject[] nightLights;

    private void Start()
    {
        Debug.Log("[Coffs-Harbor] Start");

        // On start, initialize the animation, and freeze playback
        sunAnim.Play("SunAnimation");
        sunAnim["SunAnimation"].speed = 0;

        // Disable all lights on start
        foreach (GameObject i in nightLights)
        {
            i.SetActive(false);
        }
    }
    public void Slider_Changed(float Slider_Value)
    {
        // Uncomment to debug the float of the slider value

        // Debug.Log("[Coffs-Harbor] Slider Value" + Slider_Value);

        // Sets the animation relative to the slider value
        sunAnim["SunAnimation"].normalizedTime = Slider_Value;

        // This all drives the lights turning on and off at certain times of day
        if (Slider_Value <= .375f)
        {
            foreach (GameObject i in nightLights)
            {
                i.SetActive(false);
            }
        }
        else
        {
            if (Slider_Value >= .7f)
            {
                foreach (GameObject i in nightLights)
                {
                    i.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject i in nightLights)
                {
                    i.SetActive(true);
                }
            }
        }

    }
}
