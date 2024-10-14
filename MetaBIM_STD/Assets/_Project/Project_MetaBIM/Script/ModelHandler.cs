using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class ModelHandler : MonoBehaviour
{

    public static ModelHandler Instance;


    public static ModelHandler GetInstance()
    {
        if (Instance == null)
        {
            Instance = GameObject.Find("ModelHandler").GetComponent<ModelHandler>();
        }

        return Instance;
    }





    #region Model Supplier

    [Header("Model Supplier")]
    public Supplier CurrentSupplier;
    public int SupplierIndex = 0;
    public List<Supplier> Suppliers = new List<Supplier>();



    public void CreateSupplier()
    {
        Debug.Log("CreateNewSupplier: ");
        CurrentSupplier = new Supplier("", new List<SupplierMaterial>());
    }



    public void AddSupplier()
    {
        Debug.Log("AddNewSupplier: ");

        StartCoroutine(DataProxy.GetInstace().AddSupplier(Supplier.ToJson(CurrentSupplier), AddSupplier_Callback));
    }   


    public void AddSupplier_Callback(bool _result, string _message)
    {
        Debug.Log("AddNewSupplier_Callback: ");

        if (_result)
        {
            DataProxyResponse<Supplier> response = DataProxyResponse<Supplier>.FromJson(_message);

            if (response.result)
            {
                Debug.Log("True: " + _message);
                GetSuppliers();
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }


    public void GetSuppliers()
    {
        Debug.Log("GetSuppliers: ");
        StartCoroutine(DataProxy.GetInstace().GetSuppliers("status", Config.DevelopmentStage, GetSupplier_Callback));
    }


    public void GetSupplier_Callback(bool _result, string _message)
    {
        Debug.Log("GetSupplier_Callback: ");
        SupplierIndex = 0;

        if (_result)
        {
            DataProxyResponse<Supplier> response = DataProxyResponse<Supplier>.FromJson(_message);

            if (response.result)
            {
                Suppliers = response.package;
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }

    public void OnSelectNext_Supplier()
    {
        if (Suppliers.Count > 0)
        {
            if (SupplierIndex >= Suppliers.Count)
            {
                SupplierIndex = 0;
            }
            Debug.Log("Next Supplier: " + SupplierIndex);

            CurrentSupplier = Supplier.FromJson(Supplier.ToJson( Suppliers[SupplierIndex]));
            SupplierIndex++;
        }
        else
        {
            Debug.Log("No Supplier: ");
        }
    }



    public void UpdateSupplier()
    {
        Debug.Log("UpdateSupplier: ");
        StartCoroutine(DataProxy.GetInstace().UpdateSupplier(Supplier.ToJson(CurrentSupplier), UpdateSupplier_Callback));
    }


    public void UpdateSupplier_Callback(bool _result, string _message)
    {
        Debug.Log("UpdateSupplier_Callback: ");

        if (_result)
        {
            DataProxyResponse<Supplier> response = DataProxyResponse<Supplier>.FromJson(_message);

            if (response.result)
            {
                Debug.Log("True: " + _message);
                GetSuppliers();
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }


    public void RemoveSupplier()
    {
        Debug.Log("UpdateSupplier: ");
        StartCoroutine(DataProxy.GetInstace().DeleteSupplierByGuid(CurrentSupplier.guid, RemoveSupplier_Callback));
    }


    public void RemoveSupplier_Callback(bool _result, string _message)
    {
        Debug.Log("RemoveSupplier_Callback: ");

        if (_result)
        {
            DataProxyResponse<Supplier> response = DataProxyResponse<Supplier>.FromJson(_message);

            if (response.result)
            {
                Debug.Log("True: " + _message);
                GetSuppliers();
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }

    #endregion


    #region Model Workspace

    [Header("Model Workspace")]
    public Workspace CurrentWorkspace;
    public int WorkspaceIndex = 0;
    public List<Workspace> Workspaces = new List<Workspace>();



    public void CreateWorkspace()
    {
        Debug.Log("CreateNewWorkspace: ");
        CurrentWorkspace = new Workspace();
    }



    public void AddWorkspace()
    {
        Debug.Log("AddNewWorkspace: ");

        StartCoroutine(DataProxy.GetInstace().AddWorkspace(Workspace.ToJson(CurrentWorkspace), AddWorkspace_Callback));
    }


    public void AddWorkspace_Callback(bool _result, string _message)
    {
        Debug.Log("AddNewWorkspace_Callback: ");

        if (_result)
        {
            DataProxyResponse<Workspace> response = DataProxyResponse<Workspace>.FromJson(_message);

            if (response.result)
            {
                Debug.Log("True: " + _message);
                GetWorkspaces();
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }


    public void GetWorkspaces()
    {
        Debug.Log("GetWorkspaces: ");
        StartCoroutine(DataProxy.GetInstace().GetWorkspaces("status", Config.DevelopmentStage, GetWorkspace_Callback));
    }


    public void GetWorkspace_Callback(bool _result, string _message)
    {
        Debug.Log("GetWorkspace_Callback: ");
        WorkspaceIndex = 0;

        if (_result)
        {
            DataProxyResponse<Workspace> response = DataProxyResponse<Workspace>.FromJson(_message);

            if (response.result)
            {
                Workspaces = response.package;
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }

    public void OnSelectNext_Workspace()
    {
        if (Workspaces.Count > 0)
        {
            if (WorkspaceIndex >= Workspaces.Count)
            {
                WorkspaceIndex = 0;
            }
            Debug.Log("Next Workspace: " + WorkspaceIndex);

            CurrentWorkspace = Workspace.FromJson(Workspace.ToJson(Workspaces[WorkspaceIndex]));
            WorkspaceIndex++;
        }
        else
        {
            Debug.Log("No Workspace: ");
        }
    }



    public void UpdateWorkspace()
    {
        Debug.Log("UpdateWorkspace: ");
        StartCoroutine(DataProxy.GetInstace().UpdateWorkspace(Workspace.ToJson(CurrentWorkspace), UpdateWorkspace_Callback));
    }


    public void UpdateWorkspace_Callback(bool _result, string _message)
    {
        Debug.Log("UpdateWorkspace_Callback: ");

        if (_result)
        {
            DataProxyResponse<Workspace> response = DataProxyResponse<Workspace>.FromJson(_message);

            if (response.result)
            {
                Debug.Log("True: " + _message);
                GetWorkspaces();
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }


    public void RemoveWorkspace()
    {
        Debug.Log("UpdateWorkspace: ");
        StartCoroutine(DataProxy.GetInstace().DeleteWorkspaceByGuid(CurrentWorkspace.guid, RemoveWorkspace_Callback));
    }


    public void RemoveWorkspace_Callback(bool _result, string _message)
    {
        Debug.Log("RemoveWorkspace_Callback: ");

        if (_result)
        {
            DataProxyResponse<Workspace> response = DataProxyResponse<Workspace>.FromJson(_message);

            if (response.result)
            {
                Debug.Log("True: " + _message);
                GetWorkspaces();
            }
            else
            {
                Debug.Log("False: " + _message);
            }

        }
        else
        {
            Debug.Log("Error: " + _message);
        }
    }

    #endregion


}
