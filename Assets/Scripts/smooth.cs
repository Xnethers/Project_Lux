using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smooth : MonoBehaviour
{
    public Transform FollowTarget;
    public float Radius = 3.0f;
    public float Smooth = 2.0f;

    private Transform myTransform;

    // Use this for initialization
    void Start()
    {
        if (!FollowTarget)
        { FollowTarget = GameObject.FindWithTag("Player").transform; }
        else
        {
            myTransform = transform;
            myTransform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckMargin())
        {
            transform.position = Vector3.Lerp(transform.position, FollowTarget.position, Smooth * Time.deltaTime);
        }


    }

    bool CheckMargin()
    {
        return Vector3.Distance(transform.position, FollowTarget.position) > Radius;
    }
}
