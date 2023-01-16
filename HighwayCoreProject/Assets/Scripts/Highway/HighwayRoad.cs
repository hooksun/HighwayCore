using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayRoad : MonoBehaviour, IMovingGround
{
    public GameObject prefab;
    public Rigidbody rb;
    public Vector3 direction;
    public float speed, length, playerSpeed;
    public int size;

    public Vector3 velocity{get => Vector3.back * playerSpeed;}
    
    void Start()
    {
        Vector3 pos = -direction * (length * ((float)size) * 0.5f);

        for(float i = 0; i < size; i++)
        {
            GameObject section = Instantiate(prefab, transform);
            section.transform.localPosition = pos;
            pos += direction * length;
        }
    }

    void Update()
    {
        rb.velocity = Vector3.back * playerSpeed;
        rb.position -= Vector3.forward * (speed-playerSpeed) * Time.deltaTime;
        if(rb.position.z < Player.ActivePlayer.position.z)
            rb.position += Vector3.forward * length;
    }
}
