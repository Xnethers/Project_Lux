using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryPS : MonoBehaviour {
    public bool destoryParent=false;

    void Start()
    {
        float duration = GetComponent<ParticleSystem>().main.duration;
        if(transform.parent !=null && destoryParent)
            Destroy(transform.parent.gameObject, duration);
        else
            Destroy(gameObject, duration);
    }
}
