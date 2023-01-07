using UnityEngine;

public class HighwayBarrier : MonoBehaviour
{
    public float distanceFromPlayer, visibleDistance, fadeSpeed;
    public MeshRenderer meshRenderer;
    float alpha;

    void Update()
    {
        if(Player.ActivePlayer != null)
        {
            transform.position = new Vector3(0f, 0f, Mathf.Max(Player.ActivePlayer.position.z - distanceFromPlayer, transform.position.z));
            float newAlpha = Mathf.MoveTowards(alpha, ((Player.ActivePlayer.position.z - transform.position.z) > visibleDistance?0f:1f), fadeSpeed * Time.deltaTime);
            if(alpha != newAlpha)
            {
                alpha = newAlpha;
                meshRenderer.material.SetFloat("_Alpha_multiplier", alpha);
            }
        }
    }
}
