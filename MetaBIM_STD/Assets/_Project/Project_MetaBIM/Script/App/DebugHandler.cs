using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaBIM
{
    public class DebugHandler : MonoBehaviour
    {

        public GameObject UI_MainPage;
        // Start is called before the first frame update

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }



        public void OnClick_DisableUI()
        {
            UI_MainPage.SetActive(!UI_MainPage.activeSelf);
        }




        // Command line debug

        public void OnClick_DebugCommand(string command)
        {
            if(Config.DevelopmentStage != "dev")
            {
                return;
            }


            Debug.Log("Debug Command: " + command);
            switch (command)
            {
                case "Clear":
                    Debug.ClearDeveloperConsole();
                    break;
                case "Exit":
                    Application.Quit();
                    break;
                case "Help":
                    Debug.Log("Help: \n Clear: Clear the console \n Exit: Exit the application");
                    break;
                default:
                    Debug.Log("Command not found");
                    break;
            }
        }   

    }
}
