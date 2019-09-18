using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 閃光彈 */
public class FlareDestory : MonoBehaviour
{

    public LensFlare _flare;
    public Collider closeBox;
    public float BlindTime = 5;
    public float RecoverSpeed = 0.3f;

    private float DestroyTime = 5;
    [SerializeField] private float MaxBrightness = 200;



    // Use this for initialization
    void Start()
    {

        _flare = GetComponent<LensFlare>();
        StartCoroutine(destoryflare());
    }

    // Update is called once per frame
    void Update()
    {



    }

    IEnumerator destoryflare()
    {
        yield return new WaitForSeconds(BlindTime);
        _flare.fadeSpeed = RecoverSpeed;
        closeBox.enabled = true;
        Destroy(this.gameObject, DestroyTime);
    }
}
