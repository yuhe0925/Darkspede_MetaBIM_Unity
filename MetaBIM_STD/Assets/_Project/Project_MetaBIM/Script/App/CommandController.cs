using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaBIM;

public class CommandController : MonoBehaviour
{
    public static CommandController Instance;
    public int isCommandQueueRunning = -1;  // -1 = not running, 1 = running, 0 = paused
    public string targetModelGuid;

    public List<RemoteCommand> commands = new List<RemoteCommand>();


    public int CommandRequest;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

  
    public void OnToggle_StartModelCmdListener(int _status)
    {

    }
    public void StartRemoteCommandMode(string _targetModelGuid)
    {
        Debug.Log("StartRemoteCommandMode: " + _targetModelGuid);
        targetModelGuid = _targetModelGuid;
        CommandRequest = 0;
        OnRequestCommandQueue();
    }


    public void OnRequestCommandQueue()
    {
        commands.Clear();
        StartCoroutine(DataProxy.Instance.GetRemoteCommands("targetModel", targetModelGuid, OnRequestCommandQueue_Callback));
    }


    public void OnRequestCommandQueue_Callback(bool _result, string _message)
    {
        CommandRequest++;
        Debug.Log("CommandController.OnRequestCommandQueue_Callback: " + _result);

        if (_result)
        {
            DataProxyResponse<RemoteCommand> dataProxyResponse = JsonUtility.FromJson<DataProxyResponse<RemoteCommand>>(_message);

            if (dataProxyResponse.result)
            {
                commands = dataProxyResponse.package;

                if(commands.Count == 0)
                {
                    Invoke("ProcessCommand", 1f);
                }
                else
                {
                    ProcessCommand();
                }
            }
            else
            {
                Debug.Log("Data Fail, " + _message);
            }
        }
        else
        {
            Debug.Log("API Fail, " + _message);
        }
    }


    public void ProcessCommand()
    {

        // get the lasted command by sorting the command list
        commands.Sort((x, y) => x.updated.CompareTo(y.updated));

        if (commands.Count > 0)
        {
            var cmd = commands[0];

            Debug.Log("CommandController.ProcessCommand: " + cmd.targetAction + " " + cmd.targetElement);

            switch (cmd.targetAction)
            {
                case "isolate":
                    // DO something in viewer
                    Page_BIMViewer.Instance.OnIsolateNodeObjectWithObjectName(cmd.targetElement);
                    break;
                default:
                    break;
            }

        }

        Invoke("OnProcessCommandCompelte", 1f);
    }


    public void OnProcessCommandCompelte()
    {
        if (commands.Count > 0)
        {
            // on complete
            OnDeleteCommand(commands[0]);
        }
        else
        {
            // move on next
            OnProcessNext();
        }
    }


    // NOT IN USE
    public void OnUpdateCommand(RemoteCommand _cmd, string _result = "executed")
    {

        _cmd.commandStatus = _result;
        Package package = new Package();
        package.package = RemoteCommand.ToJson(_cmd);
        StartCoroutine(DataProxy.Instance.UpdateRemoteCommand(Package.ToJson(package)));
    }




    // Delete command after it is executed
    public void OnDeleteCommand(RemoteCommand _cmd)
    {

        StartCoroutine(DataProxy.Instance.DeleteRemoteCommandByGuid(_cmd.guid, OnDeleteCommand_Callback));
    }






    public void OnDeleteCommand_Callback(bool _result, string _message)
    {
        if (_result)
        {
            // move on next
            OnProcessNext();
        }
        else
        {
            MCPopup.Instance.SetInformation("Remote Command is stoped.");
        }
    }



    public void OnProcessNext()
    {
        Debug.Log("CommandController.OnProcessNext: " + isCommandQueueRunning);

        // move on next
        if (isCommandQueueRunning == 1)
        {
            Invoke("OnRequestCommandQueue", 0.5f);
        }
        else if (isCommandQueueRunning == 0)
        {
            MCPopup.Instance.SetInformation("Remote Command is pasused.");
        }
        else if (isCommandQueueRunning == -1)
        {
            MCPopup.Instance.SetInformation("Remote Command is stoped.");
        }
        else
        {
            isCommandQueueRunning = -1;
            MCPopup.Instance.SetInformation("Remote Command is stoped.");
        }
    }
}
