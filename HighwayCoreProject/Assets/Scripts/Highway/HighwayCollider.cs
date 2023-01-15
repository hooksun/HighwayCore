using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayCollider : MonoBehaviour, IMovingGround
{
    public float speed;
    public Vector3 velocity{get => Vector3.back * speed;}
}
