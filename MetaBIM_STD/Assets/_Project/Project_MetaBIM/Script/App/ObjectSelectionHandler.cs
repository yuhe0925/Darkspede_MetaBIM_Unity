using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelectionHandler : MonoBehaviour
{
    public static ObjectSelectionHandler Instance;

    public HashSet<GameObject> SelectedObjects = new HashSet<GameObject>();

    public bool isDragSelecting = false;

    public bool isDragSelectingAllowed;

    public MouseInputUIBlocker MouseInputUIBlocker;
    [Header("UI Element")]

    public RectTransform SelectionBox;
    public Camera MainCamera;
    public LayerMask SelectionMask;


    public Vector2 StartMousePosition;
    public Vector2 LastMousePosition;

    public Vector2 minValue;
    public Vector2 maxValue;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    
    // Start is called before the first frame update
    void OnEnalbe()
    {
        OnDragSelecting_Up();
    }

    // Update is called once per frame
    void Update()
    {
        // this is a bad idea to provide a global selection handler
        if (ObjectSplitHandler.Instance.IsEditingEnabled)
        {
            return;
        }

        if (MouseInputUIBlocker.IsOnUI)
        {
            if (isDragSelecting)
            {
                isDragSelecting = false;
                SelectionBox.sizeDelta = Vector2.zero;

            }
            return;
        }

        //if (!ZoneManagement.Instance.IsEditing || ZoneManagement.Instance.IsZoneDragging)
        //{
        //    return;
        //}

        if (PageStatus.IsPanelDragging)
        {
            return;
        }

        if (!MouseInputUIBlocker.BlockedByUI && isDragSelectingAllowed)
        {
            HandlerSelection();
        }
        else
        {
            OnDragSelecting_Up();
        }
    }

    
    public void HandlerSelection()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnDragSelecting_Down();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            OnDragSelecting_Hold();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnDragSelecting_Up();
        }

    }

    public void OnDragSelecting_Down()
    {
        isDragSelecting = true;
        SelectionBox.sizeDelta = Vector2.zero;
        StartMousePosition = Input.mousePosition;
        LastMousePosition = Input.mousePosition;
    }

    public void OnDragSelecting_Hold()
    {
        isDragSelecting = true;



        if ((Vector2)Input.mousePosition != LastMousePosition)
        {
            // resize the selection box
            float width = Input.mousePosition.x - StartMousePosition.x;
            float height = Input.mousePosition.y - StartMousePosition.y;

            SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            SelectionBox.position = StartMousePosition + new Vector2(width / 2, height / 2);

            // 
            minValue = (Vector2)SelectionBox.position - (SelectionBox.sizeDelta / 2);
            maxValue = (Vector2)SelectionBox.position + (SelectionBox.sizeDelta / 2);


            // send box bound to viewer, on value change
            LastMousePosition = Input.mousePosition;
            Page_BIMViewer.Instance.OnBoxSelection(minValue, maxValue);
        }      
    }
    
    public void OnDragSelecting_Up()
    {
        isDragSelecting = false;
        SelectionBox.sizeDelta = Vector2.zero;
    }


    public void SelectObject()
    {
        
    }



    public void DeselectObject()
    {
        
    }


    public void OnToggle_EnableDraSelect(bool _selection)
    {

    }
}






