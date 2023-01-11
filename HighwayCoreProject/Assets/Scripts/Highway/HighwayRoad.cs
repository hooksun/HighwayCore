using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighwayRoad : MonoBehaviour
{
    public GameObject prefab;
    public float speed, length;
    public int size;
    
    void Start()
    {
        Vector3 pos = Vector3.back * (length * ((float)size) * 0.5f);

        for(float i = 0; i < size; i++)
        {
            GameObject section = Instantiate(prefab, transform);
            section.transform.localPosition = pos;
            pos.z += length;
        }
    }

    void Update()
    {
        transform.position -= Vector3.forward * speed * Time.deltaTime;
        if(transform.position.z < Player.ActivePlayer.position.z)
            transform.position += Vector3.forward * length;
    }
}
