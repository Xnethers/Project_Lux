using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Com.MyCompany.MyGame
{
    public class TowerUI : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider towerHealthSlider;

        [SerializeField] private TowerHealth target;

        #endregion


        #region MonoBehaviour Callbacks

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {

        }

        void Update()
        {
            // Reflect the Player Health
            if (towerHealthSlider != null)
            {
                towerHealthSlider.value = target.health / target.TowerMaxhealth;
            }
        }


        #endregion


        #region Public Methods


        #endregion


    }
}
