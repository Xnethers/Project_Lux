using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontGoThroughThings : MonoBehaviour
{

    // Careful when setting this to true - it might cause double
    // events to be fired - but it won't pass through the trigger
    public bool sendTriggerMessage = false;

    public LayerMask layerMask = -1; //make sure we aren't in this layer 
    public float skinWidth = 0.1f; //probably doesn't need to be changed 
    // public float raycastAdvance = 1f;
    private float minimumExtent;
    private float partialExtent;
    private float sqrMinimumExtent;
    private Vector3 previousPosition;
    private Rigidbody myRigidbody;
    private Collider myCollider;

    //initialize values 
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        previousPosition = myRigidbody.position;
        minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
        partialExtent = minimumExtent * (1.0f - skinWidth);
        sqrMinimumExtent = minimumExtent * minimumExtent;
    }

    void Update()
    {
        //CheckHit
        //have we moved more than our minimum extent? 
        Vector3 movementThisStep = (myRigidbody.position - previousPosition);///raycastAdvance
        float movementSqrMagnitude = movementThisStep.sqrMagnitude;

        if (movementSqrMagnitude > sqrMinimumExtent)// minimumExtent
        {
            float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            RaycastHit hitInfo;

            //check for obstructions we might have missed 
            if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
            {
                //myRigidbody.velocity=Vector3.zero;
                
                // myRigidbody.isKinematic=true;
                myCollider.SendMessage("OnTriggerEnter", hitInfo.collider);
                // if (!hitInfo.collider)
                //     return;

                // if (hitInfo.collider.isTrigger)
                //     myCollider.SendMessage("OnTriggerEnter", hitInfo.collider);
                //hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);

                //if (!hitInfo.collider.isTrigger)
                //myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent; 

            }
            Debug.DrawRay(previousPosition, movementThisStep * movementMagnitude,Color.blue);
        }

        previousPosition = myRigidbody.position;
    }
}
