using MetaBIM;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public static RoomHandler Instance;

    public RoomGenerator room;


    
    public RoomStatus roomStatus = RoomStatus.updated;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }





    public RoomSelector SelectedRoom;





    public void EnableRoom()
    {
        room.gameObject.SetActive(true);

    
    }


    public void DisableRoom()
    {
        room.gameObject.SetActive(false);

        ProjectModelHandler.Instance.RestoreActiveModels();
    }


    public List<GameObject> GetObjectsInRoom(GameObject _room)
    {
        List<GameObject> objects = new List<GameObject>();  




        return objects;
    }




    public enum RoomStatus
    {
        updated,  // room has been updated, that need to be processed
        processd, // room has been processed, not need to process again
    }

}
