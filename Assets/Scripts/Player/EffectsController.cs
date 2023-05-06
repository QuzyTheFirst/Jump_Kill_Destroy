using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : ComponentController
{
    [Header("Wind Effect")]
    [SerializeField] private ParticleSystem ps_WindEffect;
    [SerializeField] private float windEffectMinThreshold = 10f;
    [SerializeField] private float windEffectMaxThreshold = 40f;
    [SerializeField] private float windMaxRateOverTime = 25f;
    private bool doLightBlow = false, doStrongBlow = false;

    [Header("Other")]
    [SerializeField] private Camera cam;

    private void Update()
    {
        WindEffectSetting();
        LightBlowSound();
        StrongWindSound();
    }

    private void WindEffectSetting()
    {
        ParticleSystem.EmissionModule emissionModule = ps_WindEffect.emission;
        ParticleSystem.MainModule settings = ps_WindEffect.main;

        float invLerp = Mathf.InverseLerp(windEffectMinThreshold, windEffectMaxThreshold, rig.velocity.magnitude);
        emissionModule.rateOverTime = windMaxRateOverTime * invLerp;

        float dot_LookInVelocityDir = Vector3.Dot(cam.transform.forward.normalized, rig.velocity.normalized);
        dot_LookInVelocityDir = Mathf.Clamp01(dot_LookInVelocityDir);
        settings.startColor = new Color(1, 1, 1, dot_LookInVelocityDir);
    }

    private void LightBlowSound()
    {
        if(rig.velocity.magnitude >= 10f)
        {
            doLightBlow = true;
        }

        if (doLightBlow)
        {
            float targetVolume = Mathf.InverseLerp(10, 15, rig.velocity.magnitude) * 1f;
            SoundManager.Instance.ChangeSoundVolume("LightWindBlow", targetVolume);
            SoundManager.Instance.Play("LightWindBlow");
            if (rig.velocity.magnitude < 10f)
            {
                SoundManager.Instance.FadeAwayVolume("LightWindBlow", .25f);
                doLightBlow = false;
            }
        }
    }

    private void StrongWindSound()
    {
        if (rig.velocity.magnitude >= 20f)
        {
            doStrongBlow = true;
        }

        if (doStrongBlow)
        {
            float targetVolume = Mathf.InverseLerp(20, 40, rig.velocity.magnitude) * .4f;
            SoundManager.Instance.ChangeSoundVolume("StrongWindBlow", targetVolume);
            SoundManager.Instance.Play("StrongWindBlow");
            if (rig.velocity.magnitude < 20f)
            {
                SoundManager.Instance.FadeAwayVolume("StrongWindBlow", .5f);
                doStrongBlow = false;
            }
        }
    }
}
