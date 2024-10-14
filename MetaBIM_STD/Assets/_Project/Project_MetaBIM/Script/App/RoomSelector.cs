using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSelector : MonoBehaviour
{
    public WireframeDrawer wireframeDrawer;

    public bool IsSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        wireframeDrawer = GetComponent<WireframeDrawer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }





    public void OnMouseOver()
    {
        //Debug.Log("Mouse is over GameObject.");
        wireframeDrawer.IsDrawing = true;
    }

    public void OnMouseExit()
    {
        //Debug.Log("Mouse is no longer over GameObject.");
        if (!IsSelected)
        {
            wireframeDrawer.IsDrawing = false;
        }
    }


    public void OnMouseDown()
    {
        string id = gameObject.name.Split('_')[1];

        IsSelected = true;

        if (RoomHandler.Instance.SelectedRoom != null)
        {
           RoomHandler.Instance.SelectedRoom.IsSelected = false;
           RoomHandler.Instance.SelectedRoom.wireframeDrawer.IsDrawing = false;
        }

        RoomHandler.Instance.SelectedRoom = this;

        // get room information from room generator instance
        MetaBIM.Room room = MetaBIM.RoomGenerator.Instance.GetRoomById(id);

        if (room == null)
        {
            // handle error
        }

        // do something with the room information


    }



    public void OnCollisionStay(Collision collision)
    {
        
    }
}
