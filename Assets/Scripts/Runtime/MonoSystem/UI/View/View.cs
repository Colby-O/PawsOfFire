using UnityEngine;

namespace PawsOfFire.MonoSystem
{
    internal abstract class View : MonoBehaviour
    {
        /// <summary>
        /// Initialzes a view. 
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Hides a view. 
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Displays a view. 
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
