using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace MetaBIM
{
    public class HotkeyHandler : MonoBehaviour
    {

        [Header("Application Control")]
        public KeyCode Key_ControlActive;

        [Header("Application Control")]
        public KeyCode Key_CloseAPP;
        public KeyCode Key_ReloadAPP;

        [Header("UI Disable")]
        public KeyCode Key_UIDisable_P;
        public KeyCode Key_UIDisable_S;
        public GameObject Target_UIDisable;

        // Start is called before the first frame update
        void Start()
        {
  
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(Key_ControlActive))
            {
                if (Input.GetKey(Key_CloseAPP))
                {
                    AppController.Instance.OnCloseApplication();
                }
                else 
                if (Input.GetKey(Key_ReloadAPP))
                {
                    AppController.Instance.OnRestartApplicationQuietly();
                }
            }

     
        }


        private bool IsUIactive()
        {
            return EventSystem.current.currentSelectedGameObject != null;
        }
    }
}
