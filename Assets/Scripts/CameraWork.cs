using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEditor;


namespace Com.MyCompany.MyGame
{
    /// <summary>
    /// Camera work. Follow a target
    /// </summary>
    public class CameraWork : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        // The target we are following
        [SerializeField] private Transform target;

        // the height we want the camera to be above the target
        [SerializeField] private float distance = 7.0f;
        [SerializeField] private float maxdistance = 10.0f;
        // How much we 
        [SerializeField] private float mindistance = 0.0f;

        [SerializeField] private float sensitivity = 100;

        [SerializeField] private float smooth = 2;

        [Header("初始位置")]

        [SerializeField] private float x;
        [SerializeField] private float y;

        [Header("移動幅度"), Tooltip("左右幅度")]
        [SerializeField] private float Xamplitude = 2.0f;

        [Tooltip("上下幅度")]
        [SerializeField] private float Yamplitude = 2.0f;

        private Vector3 offset;

        private Quaternion rotationEuler;

        private bool Rhit;

        private float h;

        private float heightSmoothLag = 0.3f;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField] private bool followOnStart = false;
        private Transform cameraTransform;
        private bool isFollowing;

        [SerializeField] private Vector3 cameraCenter;
        private Vector3 normalCenter;

        private Vector3 _aimdir;

        #endregion

        #region Public Fields
        [HideInInspector] public Vector3 aimCenter; /* 準心位置 */
        private float normalsight; /* 正常視野 */
        [HideInInspector] public float aimsight; /* 瞄準視野 */
        [HideInInspector] public bool IsGunner;

        [HideInInspector] public Vector3 cameracenter { get { return target.position; } }

        [HideInInspector] public Vector3 aimdir { get { return _aimdir; } }
        #endregion


        #region MonoBehaviour Callbacks

        void Start()
        {
            // Start following the target if wanted.
            if (followOnStart)
            {
                OnStartFollowing();
                target.transform.localPosition = this.transform.position;
            }
            normalCenter = cameraCenter;
            normalsight = Camera.main.fieldOfView;
        }


        void LateUpdate()
        {
            // The transform target may not destroy on level load,
            // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }
            // only follow is explicitly declared
            if (isFollowing)
            {
                Apply();
            }

            cameraRay();

        }

        void Update()
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, aimsight, normalsight);
            _aimdir = target.position - cameraTransform.position;
        }
        #endregion


        #region Public Methods


        /// <summary>
        /// Raises the start following event.
        /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        /// </summary>
        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            target = GameObject.Find("cameraT").transform;
            isFollowing = true;
            // we don't smooth anything, we go straight to the right camera shot
            Cut();
        }

        public bool isaim()
        {
            if (Camera.main.fieldOfView < normalsight)
            { return true; }
            else
            { return false; }
        }

        public void Aim()
        {
            cameraCenter = aimCenter;
            Camera.main.fieldOfView -= 2;
        }

        public void unAim()
        {
            cameraCenter = normalCenter;
            Camera.main.fieldOfView += 2;
        }


        #endregion


        #region Private Methods


        /// <summary>
        /// Follow the target smoothly
        /// </summary>
        void cameraRay()
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, transform.TransformDirection(-Vector3.forward), out hit))
            {
                if (hit.collider.tag == "Wall")
                {
                    h = hit.distance / 2;
                    Rhit = true;
                }
            }
            else
            {
                Rhit = false;
            }

            if (h > distance && Rhit == true)
            {
                Rhit = false;
            }
        }
        void Apply()
        {
            // Early out if we don't have a target
            if (!target)
                return;
            if (target)
            {
                x += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
                y -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

                if (x > 360) { x -= 360; }
                else if (x < 0) { x += 360; }

                y = Mathf.Clamp(y, -Yamplitude, Yamplitude);

                distance = Mathf.Clamp(distance, mindistance, maxdistance);
                rotationEuler = Quaternion.Euler(0, x * Xamplitude, 0);
                Vector3 _offset = rotationEuler * cameraCenter + this.transform.position + new Vector3(0, -y / 10, 0);

                if (!Rhit)
                {
                    if (!isaim())
                    { offset = rotationEuler * new Vector3(0, 0, -distance) + this.transform.position + new Vector3(0, cameraCenter.y, 0); }
                    else
                    { offset = rotationEuler * new Vector3(0, 0, 1f) + this.transform.position + new Vector3(0, cameraCenter.y, 0); }
                }



                if (Rhit)
                { offset = rotationEuler * new Vector3(0, 0, -h) + this.transform.position; }

                if (photonView.IsMine)
                {
                    target.position = Vector3.Lerp(target.position, _offset, sensitivity * Time.deltaTime);
                    cameraTransform.position = Vector3.Lerp(cameraTransform.position, offset, smooth * Time.deltaTime);
                    cameraTransform.rotation = rotationEuler;
                    cameraTransform.LookAt(target);
                }
            }

        }


        /// <summary>
        /// Directly position the camera to a the specified Target and center.
        /// </summary>
        void Cut()
        {
            float oldHeightSmooth = heightSmoothLag;
            heightSmoothLag = 0.001f;
            Apply();
            heightSmoothLag = oldHeightSmooth;
        }

        /* 
        /// <summary>
        /// Sets up the rotation of the camera to always be behind the target
        /// </summary>
        /// <param name="centerPos">Center position.</param>
        void SetUpRotation(Vector3 centerPos)
        {
            Vector3 cameraPos = cameraTransform.position;
            Vector3 offsetToCenter = centerPos - cameraPos;
            // Generate base rotation only around y-axis
            Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
            Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
            cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
        }
        */


        #endregion
    }
}
