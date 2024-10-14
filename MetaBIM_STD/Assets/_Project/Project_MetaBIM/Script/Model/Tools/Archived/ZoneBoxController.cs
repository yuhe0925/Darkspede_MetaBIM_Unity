using IfcToolkit.IfcSpec;
using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// This is also a UI controller
/// </summary>
/// 
public class ZoneBoxController : MonoBehaviour
{
    public static ZoneBoxController Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    
    public GameObject ZoneBoxPrefab;
    public GameObject ZoneLevelPrefab;
    public List<ZoneBox> Zones;
    public Dictionary<int, ZoneBox> ZoneDictionary = new Dictionary<int, ZoneBox>();

    public int CurrentZone = -1;

    public Camera MainCamera;
    public LayerMask ZoneLayer;
    public LayerMask ZoneColliderLayer;

    public ZoneBoxPlane LastInteractZoneBoxPlane;

    public int ZoneSplitedIndex;

    [Header("Status")]
    public bool IsDragging;



    [Header("Birdge Data")]
    public Transform SD_Target;
    public FreeCameraNav CameraNav;

    [Header("Configure")]
    public float DragInterval = 0.5f;
    public Transform Center;


    Ray ray;
    RaycastHit hit;

    public Vector3 lastLeftClick;
 

    
    // Update is called once per frame
    void Update()
    {
        if (MouseInputUIBlocker.BlockedByUI)
        {
            return;
        }
        
        ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        
        // On Select
        if (Input.GetMouseButtonUp(0))
        {
            if (Input.mousePosition == lastLeftClick)
            {
                if (Physics.Raycast(ray, out hit, 1000f, ZoneLayer))
                {
                    OnZonePlaneSelect(hit.collider.gameObject);
                }
            }          
        }
        else if (Input.GetMouseButtonDown(0))
        {
            lastLeftClick = Input.mousePosition;
        }

        if (CameraNav.isOnRotating || CameraNav.isOnPenning)
        {
            return;
        }


        if (!IsDragging)
        {
            //Debug.Log("GetMouseButtonUp: Hover");
            if (Physics.Raycast(ray, out hit, 1000f, ZoneLayer))
            {
                OnZoneHighlight(hit.collider.gameObject);
            }
            else
            {
                CancelZoneHighlight();
            }

        }


    }



 



    public ZoneBox OnAddNewZone(Bounds _targetBound, Vector3 _offset, string _zoneName)
    {
        int nextID = Zones.Count;

        // Zone need a selectedPanel other than the first one
        if (CurrentZone > -1)
        {
            if (Zones[CurrentZone].SelectedPlaneIndex < 0)
            {
                return null;
            }
        }

       
        GameObject newZone = Instantiate(ZoneBoxPrefab);
        ZoneBox zoneBox = newZone.GetComponent<ZoneBox>();
        zoneBox.OnInitBox(_targetBound,  _offset, Center);
        zoneBox.OnResetId(nextID);
        zoneBox.ZoneName = _zoneName;

        if (Zones.Count > 0 && nextID != 0)
        {
            Zones[CurrentZone].IsPanelLockTo = true;
            zoneBox.IsPanelLocked = true;
        }
        
        // Disable editing last zone
        if (CurrentZone > -1)
        {
            Zones[CurrentZone].OnEnableEditing(false);
        }

        zoneBox.LockToPlane();

        Zones.Add(zoneBox);
        CurrentZone = nextID;

        return Zones[CurrentZone];
    }


    public void OnRemoveZone()
    {
        // remove item
        // reset index
        ZoneBox currentZone = Zones[CurrentZone];
        Destroy(currentZone.gameObject);
        Zones.RemoveAt(CurrentZone);
        CurrentZone = CurrentZone - 1;
    }


    
    public void ZoneSelected(int _index)
    {
        if (CurrentZone > -1)
        {
            Zones[CurrentZone].OnEnableEditing(false);
        }

        if (_index > -1 && _index < Zones.Count) 
        {
            Zones[_index].OnEnableEditing(true);
            CurrentZone = _index;
        }
    }

    public void OnZonePlaneSelect(GameObject _PlanelObject)
    {
        Debug.Log("OnZonePlaneSelect:" + _PlanelObject.name);

        if (Zones[_PlanelObject.GetComponent<ZoneBoxPlane>().ZoneBoxIndex].IsPanelLockTo)
        {
            Debug.Log("Plane locked");
            return;
        }

        if (LastInteractZoneBoxPlane != null)
        {
            LastInteractZoneBoxPlane.OnMouseLeave();
            LastInteractZoneBoxPlane = null;
        }

        if (_PlanelObject.GetComponent<ZoneBoxPlane>() != null)
        {
            _PlanelObject.GetComponent<ZoneBoxPlane>().OnMouseSelect();
        }
    }


    public ZoneBoxPlane GetLockedPlane(int _index)
    {
        //Debug.Log("From:    Plane = " + _index);
        //Debug.Log("Return:  Plane = " + (_index-1));
        ZoneBox box = Zones[_index-1];
        //Debug.Log("GetLockedPlane:  Plane = " + (box.SelectedPlaneIndex));
        return box.SelectablePlane[box.SelectedPlaneIndex];
    }



    public void OnZoneHighlight(GameObject _PlanelObject)
    {
        if (LastInteractZoneBoxPlane != null)
        {
            LastInteractZoneBoxPlane.OnMouseLeave();
        }
        
        if (_PlanelObject.GetComponent<ZoneBoxPlane>() != null)
        {
            LastInteractZoneBoxPlane = _PlanelObject.GetComponent<ZoneBoxPlane>();
            LastInteractZoneBoxPlane.OnMouseHover();
        }
    }



    public void HideZones()
    {
        foreach (ZoneBox zone in Zones)
        {
            zone.gameObject.SetActive(false);
        }
    }


    
    public void RevialZones()
    {
        foreach (ZoneBox zone in Zones)
        {
            zone.gameObject.SetActive(true);
        }
    }



    public void CancelZoneHighlight()
    {
        if (LastInteractZoneBoxPlane != null)
        {
            LastInteractZoneBoxPlane.OnMouseLeave();
            LastInteractZoneBoxPlane = null;
        }
    }


    public void ZoneViewSwitch(bool _on)
    {
        foreach(ZoneBox box in Zones)
        {
            box.isDrawing = _on;
            foreach(Transform tran in box.Corners)
            {
                tran.gameObject.SetActive(_on);
            }

            foreach (ZoneBoxPlane plane in box.SelectablePlane)
            {
                plane.gameObject.SetActive(_on);
            }
        }
    }
        


    public void RenderLevel()
    {
        //Debug.Log("RenderLevel");

        if (Zones.Count < 0)
        {
            return;
        }

        foreach(var zone in Zones)
        {
            zone.SetLevels(ProjectModelHandler.Instance.CurrentModel.bimModel.levels, ZoneLevelPrefab, zone.gameObject.transform);
        }
    }


    public void StopLevelRender()
    {
        //Debug.Log("StopLevelRender");

        foreach (var zone in Zones)
        {
            zone.StopLevel();
        }
    }


    // External data process

    
    public void OnRequest_LoadZoneInformation()
    {

    }


    public void OnRequest_SaveZoneInformation()
    {

    }


    public void OnRequest_ResetBox()
    {
        
    }

}
