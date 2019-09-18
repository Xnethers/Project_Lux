using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _DrawLine : MonoBehaviour
{
    public ICareerController icc;

    private LineRenderer lineRenderer;
    private TrailRenderer trailRenderer;
    private float counter;
    private float dist;

    public Transform origin;
    public Transform destination;
    public float lineDrawSpeed = 0.1f;
    public float LineDisableTime = 1f;
    public float LineDisableLerpTime = 0.2f;
    private bool isLineDisable;

    // Use this for initialization
    void Start()
    {
        // lineRenderer = GetComponent<LineRenderer>();
        // trailRenderer = GetComponent<TrailRenderer>();
        // lineRenderer.startWidth = 0.1f;
        // lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        Color c = new Color(0f, 0f, 0f, Mathf.Lerp(225, 0, Time.deltaTime));
        lineRenderer.SetColors(lineRenderer.startColor, c);
        // lineRenderer.SetPosition(0, origin.position);
        // dist = Vector3.Distance(origin.position, icc.RayAim());

        // //       if (counter < dist)
        // //       {
        // //The head is in the pointAlongLine.
        // counter += .1f / lineDrawSpeed;

        // float x = Mathf.Lerp(0, dist, counter);

        // Vector3 pointA = origin.position;
        // Vector3 pointB = icc.RayAim();

        // Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;

        // lineRenderer.SetPosition(1, pointAlongLine);

        //Debug.Log(counter);
        //       }
        if(isLineDisable){
            lineRenderer.startWidth =Mathf.Lerp(lineRenderer.startWidth,0,LineDisableLerpTime);
            lineRenderer.endWidth =Mathf.Lerp(lineRenderer.endWidth,0,LineDisableLerpTime);
        }
    }

    public void DrawLine(Vector3 point)
    {
        //初始設定
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        isLineDisable=false;

        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.material = lineRenderer.materials[0];
        dist = Vector3.Distance(origin.position, point);

        counter += .1f / lineDrawSpeed;

        float x = Mathf.Lerp(0, dist, counter);

        Vector3 pointA = origin.position;
        Vector3 pointB = point;

        Vector3 pointAlongLine = x * Vector3.Normalize(pointB - pointA) + pointA;

        lineRenderer.SetPosition(1, pointAlongLine);
        //lineRenderer.material = lineRenderer.materials[1];
        Invoke("LineDisable",LineDisableTime);
    }
    public void LineDisable(){
        // lineRenderer.enabled = false;
        isLineDisable =true;
        Destroy(gameObject,1f);
    }
}
