using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerHitEffects : MonoBehaviour
{
    //Necessary for creating QuickVolume.
    private PostProcessVolume volume;
    private int layerIndex;

    //Holds settings for effect.
    private ColorGrading colorGrading;

    //Creates graph in inspector.
    //Determines how aggressively effect will be applied.
    public AnimationCurve cgCurve;

    //Profile to use when choosing effect settings.
    public PostProcessProfile postProfile;

    public GameObject camera;

    private void Start()
    {
        //Finds PostProcessing layer to apply effect.
        //Must have PostProcessing layer added in order for effect to work properly.
        layerIndex = LayerMask.NameToLayer("PostProcessing");
        camera.GetComponent<PostProcessLayer>().volumeLayer = LayerMask.GetMask("PostProcessing");

        //Creates color grading effect and sets default settings.
        colorGrading = ScriptableObject.CreateInstance<ColorGrading>();
        colorGrading.enabled.Override(false);

        //Creates volume for effect to be applied.
        volume = PostProcessManager.instance.QuickVolume(layerIndex, 0, colorGrading);
        volume.isGlobal = true;
    }

    public void UpdateIntensity()
    {
        Debug.Log("Method not working. Vignette not currently working in VR.");
    }

    //Updates saturation based on current health.
    public void UpdateSaturation()
    {
        int curHealth = gameObject.GetComponent<Health>().GetCurrentHealth();
        int maxHealth = gameObject.GetComponent<Health>().GetMaxHealth();

        float percentHealth = (float) curHealth / maxHealth;
        float percentToGrade = -(cgCurve.Evaluate((1 - percentHealth)) * 100);

        if (percentHealth < 1f)
        {
            colorGrading.saturation.Override(percentToGrade);
            colorGrading.enabled.Override(true);
        }
        else
        {
            colorGrading.enabled.Override(false);
        }
    }


    //Old code. Currently no way to work in VR.
    /*
    //Necessary for creating QuickVolume.
    private PostProcessVolume volume;
    private int layerIndex;

    //Holds settings for effects.
    private Vignette vignette;
    private ColorGrading colorGrading;

    //Settings for vignette. Excludes mode (using classic) and intensity (set by script).
    private ColorParameter vColor;
    private Vector2Parameter vCenter;
    private FloatParameter vSmoothness;
    private FloatParameter vRoundness;
    private BoolParameter vRounded;

    //Creates graphs in inspector.
    //Determines how aggressively effects will be applied.
    public AnimationCurve vCurve;
    public AnimationCurve cgCurve;

    //Profile to use when choosing effect settings.
    public PostProcessProfile postProfile;

    public GameObject camera;

    private void Start()
    {
        //Finds PostProcessing layer to apply effect.
        //Must have PostProcessing layer added in order for effect to work properly.
        layerIndex = LayerMask.NameToLayer("PostProcessing");
        camera.GetComponent<PostProcessLayer>().volumeLayer = LayerMask.GetMask("PostProcessing");

        //Creates vignette effect and sets default settings.
        vignette = ScriptableObject.CreateInstance<Vignette>();
        vignette.enabled.Override(false);
        vignette.intensity.Override(0f);

        //Creates color grading effect and sets default settings.
        colorGrading = ScriptableObject.CreateInstance<ColorGrading>();
        colorGrading.enabled.Override(false);

        //Gets settings to use from effect profile provided.
        vColor = postProfile.GetSetting<Vignette>().color;
        vCenter = postProfile.GetSetting<Vignette>().center;
        vSmoothness = postProfile.GetSetting<Vignette>().smoothness;
        vRoundness = postProfile.GetSetting<Vignette>().roundness;
        vRounded = postProfile.GetSetting<Vignette>().rounded;

        //Sets settings to approprate values.
        vignette.color.Override(vColor);
        vignette.center.Override(vCenter);
        vignette.smoothness.Override(vSmoothness);
        vignette.roundness.Override(vRoundness);
        vignette.rounded.Override(vRounded);
        
        //Creates volume for effect to be applied.
        volume = PostProcessManager.instance.QuickVolume(layerIndex, 0, vignette, colorGrading);
        volume.isGlobal = true;
    }

    //Updates vignette intensity based on current health.
    public void UpdateIntensity()
    {
        int curHealth = gameObject.GetComponent<Health>().GetCurrentHealth();
        int maxHealth = gameObject.GetComponent<Health>().GetMaxHealth();

        float maxIntensity = 0.5f;  //Intensity will approach this value.
        float numToDivide = (float) 1 / maxIntensity;
        float percentHealth = (float) curHealth / maxHealth;
        float curIntensity = (vCurve.Evaluate((1 - percentHealth))) / numToDivide;

        if (curIntensity > 0)
        {
            vignette.intensity.Override(curIntensity);
            vignette.enabled.Override(true);
        }
        else
        {
            vignette.enabled.Override(false);
        }
    }
    
    //Updates saturation based on current health.
    public void UpdateSaturation()
    {
        int curHealth = gameObject.GetComponent<Health>().GetCurrentHealth();
        int maxHealth = gameObject.GetComponent<Health>().GetMaxHealth();

        float cutoff = 0.3f;  //Enables color grading after percentHealth is lower than this value.
        float percentHealth = (float) curHealth / maxHealth;
        float percentToGrade = -(cgCurve.Evaluate((1 - (percentHealth / cutoff))) * 100);
        
        if (percentHealth < 0.3f)
        {
            colorGrading.saturation.Override(percentToGrade);
            colorGrading.enabled.Override(true);
        }
        else
        {
            colorGrading.enabled.Override(false);
        }
    }*/
}
