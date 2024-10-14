using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MetaBIM;

public class AppUpdater : MonoBehaviour
{
    public string versionFile = "";
    // Start is called before the first frame update
    void Start()
    {
        versionFile = Application.streamingAssetsPath + "/document/update.txt";
        Invoke("ReadVersionFile", 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ReadVersionFile()
    {
        StreamReader inp_stm = new StreamReader(versionFile);
        string inp_ln = "No Updates";

        while (!inp_stm.EndOfStream)
        {
            inp_ln = inp_stm.ReadLine();
        }
        inp_stm.Close();

        Debug.Log(inp_ln);

        MCPopup.Instance.SetInformation(inp_ln, "New Version");
    }
}
