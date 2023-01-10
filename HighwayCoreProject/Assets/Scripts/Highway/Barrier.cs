using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public float fadeSpeed;
    public MeshRenderer meshRenderer;

    float alpha, alphaTarget;

    void Update()
    {
        alpha = Mathf.MoveTowards(alpha, alphaTarget, fadeSpeed * Time.deltaTime);
        meshRenderer.material.SetFloat("_Alpha_multiplier", alpha);

        if(alpha == 1f)
        {
            enabled = false;
            return;
        }
        if(alpha == 0f)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void Fade(bool fade)
    {
        gameObject.SetActive(true);
        enabled = true;
        alphaTarget = (fade?1f:0f);
    }
}
