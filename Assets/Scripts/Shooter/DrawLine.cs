using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Photon.Pun;
using DG.Tweening;

public class DrawLine : MonoBehaviour
{//,IPunObservable 
    private LineRenderer lineRenderer;
    private float counter;
    private float dist;
    public Transform origin;
    public Transform destination;
    public float lineDrawSpeed = 0.1f;

    // Use this for initialization
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // lineRenderer.startWidth = 0.45f;
        // lineRenderer.endWidth = 0.45f;
        // lineRenderer.enabled = false;
        origin = transform;

    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled && destination != null)
        {
            lineRenderer.SetPosition(0, origin.position);
            dist = Vector3.Distance(origin.position, destination.position);

            // if (counter < dist)
            // {
            //     The head is in the pointAlongLine.
            counter += .1f / lineDrawSpeed;

            float x = Mathf.Lerp(0, dist, counter);

            Vector3 pointA = origin.position;
            Vector3 pointB = destination.position;

            Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;

            lineRenderer.SetPosition(1, pointAlongLine);

            //     Debug.Log(counter);
            // }
        }

    }
    //[PunRPC]
    public void SetLineEnabled(bool enabled)
    {
        lineRenderer.enabled = enabled;
    }
    //[PunRPC]
    public void Disappear()
    {
        lineRenderer.startWidth = Mathf.Lerp(lineRenderer.startWidth, 0, 1f);
        lineRenderer.endWidth = Mathf.Lerp(lineRenderer.endWidth, 0, 1f);
    }
    //[PunRPC]
    public void Appear(float originWidth)
    {
        lineRenderer.startWidth = Mathf.Lerp(lineRenderer.startWidth, originWidth, 1f);
        lineRenderer.endWidth = Mathf.Lerp(lineRenderer.endWidth, originWidth, 1f);
    }

    // void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //      if (stream.IsWriting)
    //     {
    //         // We own this player: send the others our data
    //         stream.SendNext(origin);
    //         stream.SendNext(destination);
    //         stream.SendNext(lineRenderer);
    //     }
    //     else
    //     {
    //         // Network player, receive data
    //         this.origin = (Transform)stream.ReceiveNext();
    //         this.destination = (Transform)stream.ReceiveNext();
    //         this.lineRenderer = (LineRenderer)stream.ReceiveNext();
    //     }
    //     throw new System.NotImplementedException();
    // }
}
