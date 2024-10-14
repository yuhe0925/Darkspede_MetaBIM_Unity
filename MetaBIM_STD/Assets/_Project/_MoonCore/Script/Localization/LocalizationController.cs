using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
using System.IO;

public class LocalizationController : MonoBehaviour
{

    #region Instance 
    public static LocalizationController Instance;
    private static string InstanceObjectName = "MC_UIFrame_Runtime";   // need move this config

    public ExportDrive Drive = ExportDrive.D;
    public string BuildPath = Application.streamingAssetsPath;

    public bool IsLocalizationEnable = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public static void GetInstance()
    {
        if (Instance == null)
        {
            Instance = GameObject.Find(InstanceObjectName).GetComponent<LocalizationController>();
        }
    }

    #endregion

    public enum SelectedLocation
    {
        EN = 0,
        CH = 1
    };

    public enum ExportDrive {
        C = 0,
        D = 1,
        E = 2,
        F = 3,
    }
    

    public SelectedLocation CurrentLocation = SelectedLocation.EN;
    public bool KeepTranslatedData = true;
    public List<LocalizationItem> Items;
    public List<LocalString> LocalStringList ;
    public LocalizationTable Table;


    public static bool CheckExistItemID(string _id)
    {
        GetInstance();

        foreach (LocalizationItem item in Instance.Items)
        {
            // TODO, checking item ?
            if (item.ItemID == _id)
            {
                return true;
            }
        }

        return false;
    }


    public void SetLocaltion(SelectedLocation Location)
    {
        if (IsLocalizationEnable)
        {
            switch (Location)
            {
                case SelectedLocation.EN:
                    StartCoroutine(SwitchLocationToEN());
                    break;
                case SelectedLocation.CH:
                    StartCoroutine(SwitchLocationToCH());
                    break;
                default:
                    StartCoroutine(SwitchLocationToEN());
                    break;
            }
        }
    }




    IEnumerator SwitchLocationToEN()
    {
        int Count = 1;

        foreach (LocalizationItem item in Items)
        {
            /*
              var newText = Table.EN.Find(i => i.ID == item.ItemID).Text;
              if (item.Type == LocalizationItem.TextType.text)
              {
                  item.TargetText.text = newText;
              }
              else if (item.Type == LocalizationItem.TextType.input)
              {
                  item.TargetField.text = newText;
              }
             */
            if (!item.LocalString.Ignored && item.LocalString.EN != "")
            {
                item.SetLocalize("EN");
            }
            Count++;
            yield return Count;
        }


        CurrentLocation = SelectedLocation.EN;
    }



    IEnumerator SwitchLocationToCH()
    {
        int Count = 1;
        foreach (LocalizationItem item in Items)
        {
            /*
            var newText = Table.CH.Find(i => i.ID == item.ItemID).Text;
            if (item.Type == LocalizationItem.TextType.text)
            {
                item.TargetText.text = newText;
            }
            else if (item.Type == LocalizationItem.TextType.input)
            {
                item.TargetField.text = newText;
            }
            */
            if (!item.LocalString.Ignored && item.LocalString.CH != "")
            {
                item.SetLocalize("CH");
            }
            Count++;
            yield return Count;
        }
        CurrentLocation = SelectedLocation.CH;
    }

}


[Serializable]
public class LocalizationTable
{
    public string Config = "EN, CH";
    public string FilePath = Application.streamingAssetsPath + "/Resource/";

    public List<LocalString> EN;
    public List<LocalString> CH;

    public LocalizationTable()
    {
        EN = new List<LocalString>();
        CH = new List<LocalString>();
    }


    public void ToCSV()
    {
        string driveSuffix = "D";

        switch (LocalizationController.Instance.Drive)
        {
            case LocalizationController.ExportDrive.C:
                driveSuffix = "C:\\";
                break;
            case LocalizationController.ExportDrive.D:
                driveSuffix = "D:\\";
                break;
            case LocalizationController.ExportDrive.E:
                driveSuffix = "E:\\";
                break;
            case LocalizationController.ExportDrive.F:
                driveSuffix = "F:\\";
                break;
            default:
                driveSuffix = "D:\\";
                break;
        }
        
        
        try
        {
            string path = FilePath;
            string fileName = "LocalizationTable.csv";
            File.WriteAllText(path + fileName, "id,EN,CH\n");

            Debug.Log("Export Table to: " + path + fileName);

            List<string> ids = new List<string>();
            foreach (LocalString ls in EN)
            {
                if (!ids.Contains(ls.ID))
                {
                    ids.Add(ls.ID);
                }
            }

            foreach (LocalString ls in CH)
            {
                if (!ids.Contains(ls.ID))
                {
                    ids.Add(ls.ID);
                }
            }
            Debug.Log(ids.Count + " items");
            using (StreamWriter sw = File.AppendText(path + fileName))
            {
                foreach (string id in ids)
                {
                    string english = "Not translated";
                    string chinese = "未翻译";
                    LocalString enls = EN.Find(i => i.ID == id);
                    LocalString chls = CH.Find(i => i.ID == id);
                    if (enls != null) english = enls.EN;
                    if (chls != null) chinese = chls.CH;

                    //Debug.Log(id + "," + english + "," + chinese);
                    sw.WriteLine(id + "," + english + "," + chinese);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public static LocalizationTable FromCSV(string _filePath)
    {
        string driveSuffix = "D";

        switch (LocalizationController.Instance.Drive)
        {
            case LocalizationController.ExportDrive.C:
                driveSuffix = "C:\\";
                break;
            case LocalizationController.ExportDrive.D:
                driveSuffix = "D:\\";
                break;
            case LocalizationController.ExportDrive.E:
                driveSuffix = "E:\\";
                break;
            case LocalizationController.ExportDrive.F:
                driveSuffix = "F:\\";
                break;
            default:
                driveSuffix = "D:\\";
                break;
        }

        
        LocalizationTable table = new LocalizationTable();

        Debug.Log("Load Table From: " + _filePath);

        using (StreamReader sr = new StreamReader(driveSuffix + _filePath))
        {
            string ln = sr.ReadLine(); // Skip the first row
            while ((ln = sr.ReadLine()) != null)
            {

                if (ln == "")
                {
                    break;
                }
                else
                {
                    var list = ln.Split(',');

                    if (list[1] != "") // English String
                    {
                        LocalString ls = new LocalString();
                        ls.ID = list[0];
                        ls.Location = "EN";
                        ls.EN = list[1];
                        table.EN.Add(ls);
                    }
                    if (list[2] != "") // Chinese String
                    {
                        LocalString ls = new LocalString();
                        ls.ID = list[0];
                        ls.Location = "CH";
                        ls.CH = list[2];
                        table.CH.Add(ls);
                    }
                }
            }
        }
        return table;
    }

}
