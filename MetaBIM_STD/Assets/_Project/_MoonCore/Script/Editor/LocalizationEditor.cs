using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
using System.IO;

public class LocalizationEditor : MonoBehaviour
{

    public static bool CheckExistItemID(string _id)
    {
        LocalizationController.GetInstance();

        foreach (LocalizationItem item in LocalizationController.Instance.Items)
        {
            // TODO, checking item ?
            if (item.ItemID == _id)
            {
                return true;
            }
        }

        return false;
    }





    [MenuItem("MC UIFrame/Localization/Collect All Items")]
    static void CollectAllItems()
    {
        Debug.Log("CollectAllItems");
        LocalizationController.GetInstance();

        LocalizationController.Instance.Items.Clear();
        LocalizationController.Instance.LocalStringList.Clear();

        LocalizationItem[] obs = GameObject.FindObjectsOfType<LocalizationItem>(true);

        foreach (LocalizationItem item in obs)
        {
            // TODO, checking item ?
            if (item.Type != LocalizationItem.TextType.icon)
            {
                item.SetDefaultText();
                LocalizationController.Instance.Items.Add(item);
                LocalizationController.Instance.LocalStringList.Add(item.LocalString);
            }
        }

        Debug.Log(obs.Length + " items collected");

    }


    [MenuItem("MC UIFrame/Localization/Reset Item By EN (WIP)")]
    static void ResetItemByEN()
    {
    
        Debug.Log("ResetItemByEN");
        LocalizationController.GetInstance();

        LocalizationController.Instance.Items.Clear();
        LocalizationController.Instance.LocalStringList.Clear();

        LocalizationItem[] obs = GameObject.FindObjectsOfType<LocalizationItem>(true);

        foreach (LocalizationItem item in obs)
        {
            if (item != null)
            {
                if (item.Type != LocalizationItem.TextType.icon)
                {
                    LocalizationController.Instance.Items.Add(item);
                    item.SetToCreatedText_EN();
                }
            }
        }

    }





    [MenuItem("MC UIFrame/Localization/Export Localization Table")]
    static void ExportLocalizationTable()
    {
        LocalizationController.GetInstance();

        if (LocalizationController.Instance.Items.Count > 0)
        {
            // Create File
            if (LocalizationController.Instance.Table == null)
            {
                LocalizationController.Instance.Table = new LocalizationTable();
            }

            if (LocalizationController.Instance.KeepTranslatedData)
            {
                if (LocalizationController.Instance.CurrentLocation == LocalizationController.SelectedLocation.EN)
                {
                    LocalizationController.Instance.Table.EN.Clear();
                }
                else if (LocalizationController.Instance.CurrentLocation == LocalizationController.SelectedLocation.CH)
                {
                    LocalizationController.Instance.Table.CH.Clear();
                }
            }
            else
            {
                LocalizationController.Instance.Table.EN.Clear();
                LocalizationController.Instance.Table.CH.Clear();
            }


            try
            {
                foreach (LocalizationItem item in LocalizationController.Instance.Items)
                {
                    LocalString ls = new LocalString();
                    ls.ID = item.ItemID;
                    ls.Location = LocalizationController.Instance.CurrentLocation.ToString();
                    // Get value 
                    if (item.Type == LocalizationItem.TextType.text)
                    {
                        ls.EN = item.TargetText.text;
                    }
                    else if (item.Type == LocalizationItem.TextType.input)
                    {
                        ls.EN = item.TargetField.text;
                    }


                    // Write into cell
                    if (LocalizationController.Instance.CurrentLocation == LocalizationController.SelectedLocation.EN)
                    {
                        LocalizationController.Instance.Table.EN.Add(ls);
                    }
                    else if (LocalizationController.Instance.CurrentLocation == LocalizationController.SelectedLocation.CH)
                    {
                        LocalizationController.Instance.Table.CH.Add(ls);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Try Collect All Items first. " + ex.Message);
            }
            // Export to csv;


            // Save file to location
            LocalizationController.Instance.Table.ToCSV();
        }
    }




    [MenuItem("MC UIFrame/Localization/Import Localization Table")]
    static void ImportLocalizationTable()
    {
        LocalizationController.GetInstance();
        LocalizationController.Instance.Table = LocalizationTable.FromCSV(LocalizationController.Instance.Table.FilePath + "LocalizationTable.csv");

    }



    [MenuItem("MC UIFrame/Localization/Switch Location To Default")]
    static void SwitchLocationToDefault()
    {
        CollectAllItems();

        foreach (LocalizationItem item in LocalizationController.Instance.Items)
        {
            if (!item.LocalString.Ignored)
            {
                item.SetDefaultText();
            }
        }
        LocalizationController.Instance.CurrentLocation = LocalizationController.SelectedLocation.EN;
    }



    [MenuItem("MC UIFrame/Localization/Switch Location To EN")]
    static void SwitchLocationToEN()
    {
        LocalizationController.GetInstance();

        foreach (LocalizationItem item in LocalizationController.Instance.Items)
        {
            if (!item.LocalString.Ignored && item.LocalString.EN != "")
            {
                item.SetLocalize("EN");
            }
        }
        LocalizationController.Instance.CurrentLocation = LocalizationController.SelectedLocation.EN;
    }



    [MenuItem("MC UIFrame/Localization/Switch Location To CH")]
    static void SwitchLocationToCH()
    {
        LocalizationController.GetInstance();
        foreach (LocalizationItem item in LocalizationController.Instance.Items)
        {
            if (!item.LocalString.Ignored && item.LocalString.CH != "")
            {
                item.SetLocalize("CH");
            }

        }
        LocalizationController.Instance.CurrentLocation = LocalizationController.SelectedLocation.CH;
    }




    [MenuItem("MC UIFrame/Localization/Open Table File Folder")]
    static void OpenTableFilePath()
    {
        LocalizationController.GetInstance();
        Application.OpenURL(LocalizationController.Instance.Table.FilePath);
    }



    [MenuItem("MC UIFrame/Open Build Path")]
    static void OpenBuildPath()
    {
        Application.OpenURL(LocalizationController.Instance.BuildPath);
    }
}
