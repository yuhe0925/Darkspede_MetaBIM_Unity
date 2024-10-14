using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MetaBIM;

public class UIBlock_Project_CollaborationItem : MonoBehaviour
{
    public Collaboration Item;

    public MC_GetWebIcon UserIcon;
    public GameObject SelectionObject;

    public TextMeshProUGUI Text_CollaborationEmail;
    public TextMeshProUGUI Text_CollaborationName;

    public GameObject AcceptButton;

    // Start is called before the first frame update
    void Start()
    {
        OnDeselected();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetBlock(Collaboration _item)
    {
        AcceptButton.SetActive(false);

        Item = _item;

        //StartCoroutine(DataProxy.Instance.GetUserByGuid(Item.collaborator, SetBlockCallback));
    }


    public void SetBlockCallback(bool success, string message)
    {
        if (success)
        {
            Profile user = JsonUtility.FromJson<DataProxyResponse<Profile>>(message).package[0];
            Page_Project.Instance.CollaborationUsers.Add(user);
            UserIcon.ImageUrl = user.iconUrl;
            UserIcon.SetBlock();

            Text_CollaborationEmail.text = string.Format("{0} ({1})", user.email, Item.collaborationStatus);
            Text_CollaborationName.text = string.Format("[{0}] {1}", Item.permission, user.fullName);

            if(user.guid == AppController.Instance.CurrentProfile.guid)
            {
                if(Item.collaborationStatus == "pending")
                {
                    AcceptButton.SetActive(true);
                }
            }
        }
        else
        {
            MCPopup.Instance.SetWarning("Load project member failed");
        }
    }

    public void OnSelected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(true);
        }
    }

    public void OnDeselected()
    {
        if (SelectionObject != null)
        {
            SelectionObject.SetActive(false);
        }
    }
}
