using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.z = Player.ActivePlayer.position.z;
        transform.position = newPos;
    }
}
