using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MultiSegmentProjectile : Projectile
//只限Box
{
    [Header("===== MultiSegment Settings =====")]
    public BoxCollider col_range;
    public float duration = 3;//持續時間
    public float Intervals = 0.1f;//間隔時間
    public float DelayedLaunch = 0.0f;//延遲發動時間
    public LayerMask mask;//目標對象layer
    public Collider[] cols;//目標對象collider
    public float Damage;

    public override void Awake()
    {
        col_range = GetComponent<BoxCollider>();
    }

    void Start()
    { }

    public override void OnEnable()
    {
        cols = Physics.OverlapBox(transform.position, col_range.size, transform.rotation, mask);
        base.OnEnable();
        Debug.LogError(am);
        Invoke("close", duration);
        InvokeRepeating("Invoke_DoMultiDamage", DelayedLaunch, Intervals);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        CancelInvoke();
    }

    void Update()
    {
        cols = Physics.OverlapBox(transform.position, col_range.size *5, Quaternion.identity, mask);
        // Invoke("Invoke_DoMultiDamage", DelayedLaunch);
    }
    public void Invoke_DoMultiDamage()
    {
        foreach (Collider col in cols)
        {
            if (col.tag == this.tag)
            {
                Debug.Log("col.tag == this.tag" + col.name);
                // isHit=true;
                return;
            }
            if (col.GetComponent<DamageHandler>() == null)
            {
                return;
            }   
            col.SendMessageUpwards("TryDoDamage", GetATK());
        }
    }

    void close()
    { 
        transform.parent.gameObject.SetActive(false); 
        am.SendMessage("OnForceAttackExit");
    }
    public void find()
    {
        cols = Physics.OverlapBox(transform.position, col_range.size / 2, Quaternion.identity, mask);
    }

    void OnDrawGizmos()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, col_range.size);
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(Vector3.zero, col_range.size);
    }

}
