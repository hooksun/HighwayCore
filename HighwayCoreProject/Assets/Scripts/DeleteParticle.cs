using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteParticle : MonoBehaviour
{
    void Start()
    {
        Invoke("destroyObj", .2f);
    }

    void destroyObj(){
        Destroy(gameObject);
    }
}
