using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public enum MoveState
        {
            idle,
            isMove,
            dead
        }

        public MoveState MS;
        public float deadTime = 5f;
        public Text textTag;

        #endregion


        #region Private Fields
        [SerializeField] private AudioClip walksound;

        private CharacterController _player;
        [SerializeField] private Transform _groundChecker;
        [SerializeField] private LayerMask Ground;
        [SerializeField] private float GroundDistance = 0.2f;
        [SerializeField] private float speed = 3.0f;
        [SerializeField] private float JumpHeight = 2;
        [SerializeField] Vector3 _velocity;

        [SerializeField] private Transform mainCameraT;
        private Transform CameraD;
        GameObject CameraD_object;
        float distance = 5.0f;
        private float rotationVelocity;
        [SerializeField] bool is_grounded;
        private AudioSource _audiosource;
        private PlayerAnimatorManager PAM;
        private bool isDead;
        private float tempDeadTime;
        

        Vector3 j;

        #endregion


        // Use this for initialization

        #region MonoBehaviour CallBacks
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }
        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
         {
             this.CalledOnLevelWasLoaded(scene.buildIndex);
         };
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red>Missing</Color> CameraWork Component on playerPrefab.", this);
            }

            _player = GetComponent<CharacterController>();
            _audiosource = GetComponent<AudioSource>();

            //Camera轉向
            mainCameraT = Camera.main.gameObject.transform;
            CameraD_object = new GameObject();
            CameraD_object.transform.parent = transform;
            CameraD_object.transform.localPosition = Vector3.zero;
            CameraD_object.name = "Direction";
            CameraD = CameraD_object.transform;
    

            StartCoroutine(FootSound());

            PAM = GetComponent<PlayerAnimatorManager>();
            foreach(Player p in PhotonNetwork.PlayerList){
                Debug.Log(photonView.Owner.NickName);
                PlayerManager[] PMs=FindObjectsOfType<PlayerManager>();
                foreach(PlayerManager pm in PMs){
                    if(p.NickName==pm.photonView.Owner.NickName){
                        SetTag(p,pm.photonView.gameObject);
                    }
                }
            }
            textTag.text ="Tag:" + transform.tag.ToString();
        }

        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
        }


        // Update is called once per frame
        void Update()
        {
            j = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

            if (Health <= 0f)
            {
                //GameManager.Instance.LeaveRoom();
                MS = MoveState.dead;
            }

            /*按鍵管理 */
            //移動
            float Vertical = Input.GetAxis("Vertical");
            float v = Mathf.Clamp(Vertical, -1f, 1f);
            float Horizontal = Input.GetAxis("Horizontal");
            float h = Mathf.Clamp(Horizontal, -1f, 1f);
            //跳躍
            if (Input.GetKeyDown(KeyCode.Space))
            { _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y); }
            /*按鍵管理 */


            if (photonView.IsMine)
            {
                //人物跟隨攝影機方向
                if (mainCameraT)
                { 
                    CameraD.eulerAngles = new Vector3(0, mainCameraT.eulerAngles.y, 0); 
                    if(PAM.animator.GetBool("TPS")){
                        float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, mainCameraT.eulerAngles.y, ref rotationVelocity,0.01f);
                        transform.eulerAngles = new Vector3(0, yAngle, 0);
                    }
                    
                }
                //跳躍
                is_grounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
                if (is_grounded && _velocity.y < 0)
                { _velocity.y = 0f; }
                _velocity.y += Physics.gravity.y * Time.deltaTime;
                _player.Move(_velocity * Time.deltaTime);

                //上下左右鍵方向
                if(!PAM.animator.GetBool("TPS")){
                    if (v > 0)
                    { SmoothRotation(CameraD.eulerAngles.y); }
                    else if (v < 0)
                    { SmoothRotation(CameraD.eulerAngles.y - 180); }

                    if (h > 0)
                    { SmoothRotation(CameraD.eulerAngles.y + 90); }
                    else if (h < 0)
                    { SmoothRotation(CameraD.eulerAngles.y - 90); }
                }

                switch (MS)
                {
                    case MoveState.idle:
                        {
                            if (Vertical != 0)
                            {
                                MS = MoveState.isMove;
                            }
                            else if (Horizontal != 0)
                            {
                                MS = MoveState.isMove;
                            }
                        }
                        break;

                    case MoveState.isMove:
                        {
                            _player.Move(CameraD.forward * v * Time.deltaTime * speed);
                            _player.Move(CameraD.right * h * Time.deltaTime * speed);

                            if (v == 0 && h == 0)
                            {
                                MS = MoveState.idle;
                            }

                        }
                        break;
                    case MoveState.dead:
                        {
                            if(!isDead){
                                PAM.Ani_Trigger("dead");
                                isDead=true;
                            }
                                
                            tempDeadTime+=Time.deltaTime;
                            if(tempDeadTime > deadTime)
                            {
                                if (gameObject.tag == "Red")
                                    photonView.RPC("ReLive",RpcTarget.All,RandomPosition(0,10),RandomPosition(0,10));
                                if(gameObject.tag == "Blue")
                                    photonView.RPC("ReLive",RpcTarget.All,RandomPosition(-30,-20),RandomPosition(-30,-20));
                                tempDeadTime=0;
                            }
                            //
                        }
                        break;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (other.name.Contains("bullet"))
            {
                Health -= 0.1f * Time.deltaTime;
            }
            if (other.name.Contains("Sword"))
            { Health -= 1f * Time.deltaTime; }
        }
        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are touching the player
        /// </summary>
        /// <param name="other">Other.</param>
        void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine)
            {
                return;
            }
            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (other.name.Contains("bullet"))
            {
                Health -= 0.1f * Time.deltaTime;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.

        }
        private float RandomPosition(float min,float max){
            float x=Random.Range(min,max);
            return x;
        }

        #endregion

        #region PUN Callbacks
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                this.Health = (float)stream.ReceiveNext();
            }
        }
        [PunRPC]
        private void ReLive(float x,float z){
            
            MS = MoveState.idle;
            PAM.Ani_Trigger("idle");
            Health=1;
            isDead=false;
            transform.localPosition = new Vector3(x,0,z);
        }
        [PunRPC]
        private void SetTag(Player p,GameObject go){
            if(p.GetTeam() == PunTeams.Team.red)
                go.transform.tag = "Red";
            else if(p.GetTeam() == PunTeams.Team.blue)
                go.transform.tag = "Blue";
        }

        #endregion



        #region Custom

        /// <summary>
        /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
        /// </summary>

        IEnumerator FootSound()
        {
            //Debug.Log(_player.velocity.magnitude);
            while (true)
            {
                if (is_grounded && MS == MoveState.isMove)
                {
                    _audiosource.PlayOneShot(walksound);
                    yield return new WaitForSeconds(walksound.length);
                }
                else
                { yield return null; }
            }
        }

        public void SmoothRotation(float a)
        {
            float y = 3.0f;
            float rotateSpeed = 0.05f;
            transform.eulerAngles = new Vector3(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, a, ref y, rotateSpeed), 0);
        }

        void CalledOnLevelWasLoaded(int level)
        {

            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (this != null && !Physics.Raycast(j, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }




        #endregion
    }
}
