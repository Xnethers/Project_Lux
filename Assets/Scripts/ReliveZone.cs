using System.Collections;
using UnityEngine;

public class ReliveZone : MonoBehaviour
{
    public enum team
    { red, blue }
    public team Team;
    public Vector3 size = new Vector3(1, 1, 1);

    public float MinX
    {
        get { return transform.position.x - size.x / 2; }
    }

    public float MaxX
    {
        get { return transform.position.x + size.x / 2; }
    }

    public float MinZ
    {
        get { return transform.position.z - size.z / 2; }
    }

    public float MaxZ
    {
        get { return transform.position.z + size.z / 2; }
    }

    public float Y
    {
        get { return transform.position.y; }
    }


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Callback to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.matrix = rotationMatrix;
        if (Team == team.red)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawCube(Vector3.zero, size);
        }
        else if (Team == team.blue)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
            Gizmos.DrawCube(Vector3.zero, size);
        }
    }
}
