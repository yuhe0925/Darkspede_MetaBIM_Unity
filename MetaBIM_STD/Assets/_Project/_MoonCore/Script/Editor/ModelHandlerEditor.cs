using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading.Tasks;
using System.IO;

public class ModelHandlerEditor : MonoBehaviour
{

    #region Model Supplier
    [MenuItem("ModelHandler/Supplier/0_Create Supplier")]
    public static void CreateModel_Supplier()
    {
        ModelHandler.GetInstance().CreateSupplier();
    }

    [MenuItem("ModelHandler/Supplier/1_Add Supplier")]
    public static void AddModel_Supplier()
    {
        ModelHandler.GetInstance().AddSupplier();
    }

    [MenuItem("ModelHandler/Supplier/2_Get Suppliers")]
    public static void GetModels_Supplier()
    {
        ModelHandler.GetInstance().GetSuppliers();
    }

    [MenuItem("ModelHandler/Supplier/3_Select Next Supplier")]
    public static void SelectNext_Supplier()
    {
        ModelHandler.GetInstance().OnSelectNext_Supplier();
    }

    [MenuItem("ModelHandler/Supplier/4_Update Supplier")]
    public static void UpdateModel_Supplier()
    {
        ModelHandler.GetInstance().UpdateSupplier();
    }

    [MenuItem("ModelHandler/Supplier/5_Remove Supplier")]
    public static void RemoveModel_Supplier()
    {
        ModelHandler.GetInstance().RemoveSupplier();
    }

    #endregion


    #region Model Workspace
    [MenuItem("ModelHandler/Workspace/0_Create Workspace")]
    public static void CreateModel_Workspace()
    {
        ModelHandler.GetInstance().CreateWorkspace();
    }

    [MenuItem("ModelHandler/Workspace/1_Add Workspace")]
    public static void AddModel_Workspace()
    {
        ModelHandler.GetInstance().AddWorkspace();
    }

    [MenuItem("ModelHandler/Workspace/2_Get Workspaces")]
    public static void GetModels_Workspace()
    {
        ModelHandler.GetInstance().GetWorkspaces();
    }

    [MenuItem("ModelHandler/Workspace/3_Select Next Workspace")]
    public static void SelectNext_Workspace()
    {
        ModelHandler.GetInstance().OnSelectNext_Workspace();
    }

    [MenuItem("ModelHandler/Workspace/4_Update Workspace")]
    public static void UpdateModel_Workspace()
    {
        ModelHandler.GetInstance().UpdateWorkspace();
    }

    [MenuItem("ModelHandler/Workspace/5_Remove Workspace")]
    public static void RemoveModel_Workspace()
    {
        ModelHandler.GetInstance().RemoveWorkspace();
    }

    #endregion




}