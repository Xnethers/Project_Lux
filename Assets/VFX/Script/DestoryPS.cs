using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryPS : MonoBehaviour {


    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
        //float duration = GetComponent<ParticleSystem>().duration;
        //Destroy(gameObject, duration);
    }
}
