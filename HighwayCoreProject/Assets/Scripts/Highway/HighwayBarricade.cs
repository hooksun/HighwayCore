using UnityEngine;

public class HighwayBarricade : MonoBehaviour
{
    public float distanceFromPlayer;

    void Update()
    {
        if(Player.ActivePlayer != null)
        {
            transform.position = new Vector3(0f, 0f, Mathf.Max(Player.ActivePlayer.position.z - distanceFromPlayer, transform.position.z));
        }
    }
}
