using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float _time;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _time -= Time.deltaTime;
        if (_time <= 0)
        { Destroy(this.gameObject); }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Body"))
            Destroy(this.gameObject);
    }
}
