using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    public LineRenderer[] Flames;
    public float FlameLength, FlameSpeed;
    public Audio JetpackAudio;

    float flameLength;
    bool enableFlames;

    public void Activate()
    {
        enabled = false;
        flameLength = 0f;
        enableFlames = false;
        EnableFlames(false);
    }

    public void Enable(bool enable)
    {
        if(enableFlames == enable)
            return;
        enableFlames = enable;
        if(enable)
        {
            enabled = true;
            JetpackAudio.Play();
            EnableFlames(true);
            return;
        }
        JetpackAudio.Stop();
    }

    void Update()
    {
        float flameTarget = enableFlames?FlameLength:0f;
        flameLength = Mathf.MoveTowards(flameLength, flameTarget, FlameSpeed * Time.deltaTime);
        SetFlameLength(flameLength);
        if(!enableFlames && flameLength == flameTarget)
        {
            enabled = false;
            EnableFlames(false);
        }
    }

    void EnableFlames(bool enable)
    {
        foreach(LineRenderer line in Flames)
            line.gameObject.SetActive(enable);
    }

    void SetFlameLength(float length)
    {
        foreach(LineRenderer line in Flames)
        {
            line.SetPosition(0, line.transform.position);
            line.SetPosition(1, line.transform.position - line.transform.forward * length);
        }
    }
}
