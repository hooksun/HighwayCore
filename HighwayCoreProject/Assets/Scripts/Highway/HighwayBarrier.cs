using UnityEngine;

public class HighwayBarrier : MonoBehaviour
{
    public float distanceFromPlayer, visibleDistance, fadeSpeed;
    public MeshRenderer meshRenderer;
    public bool visible{get; set;}
    float alpha;

    void Update()
    {
        if(Player.ActivePlayer != null && Time.deltaTime > 0f)
        {
            transform.position = Vector3.forward * (Player.ActivePlayer.score - distanceFromPlayer);
            float alphaTarget = ((visible || (Player.ActivePlayer.position.z - transform.position.z) < visibleDistance)?1f:0f);
            float newAlpha = Mathf.MoveTowards(alpha, alphaTarget, fadeSpeed * Time.deltaTime);
            if(alpha != newAlpha)
            {
                alpha = newAlpha;
                meshRenderer.material.SetFloat("_Alpha_multiplier", alpha);
                meshRenderer.enabled = alpha > 0f;
            }
        }
    }
}
