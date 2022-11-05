using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement Movement;
    public PlayerAim Aim;
    public PlayerStatus Status;

    public float TrailTime;
    public Vector3 positionOffset;
    [HideInInspector] public Vector3 trailPosition;

    void Update()
    {
        StartCoroutine(Trail(position));
    }

    IEnumerator Trail(Vector3 pos)
    {
        yield return new WaitForSeconds(TrailTime);
        trailPosition = pos;
    }

    public Vector3 position{get => transform.position + positionOffset;}
}
