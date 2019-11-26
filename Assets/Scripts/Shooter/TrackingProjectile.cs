using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrackingProjectile : MonoBehaviourPunCallbacks
{
    [Header("===== Tracking Projectile  Settings =====")]
    public Transform target;
    public Rigidbody rb;
    public float speed;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.LookAt(target);
        rb.velocity = rb.transform.forward * speed;
    }

    public void setTrack(Transform t, float s)
    {
        target = t;
        speed = s;
    }
}