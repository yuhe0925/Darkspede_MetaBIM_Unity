using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIBlock_BimViewer_TransformItem : MonoBehaviour
{
    public Transform Item;
    public Vector3 Position;
    public Vector3 Rotation;

    public Vector3 Position_New;
    public Vector3 Rotation_New;

    public bool isReady = false;

    public TextMeshProUGUI Text_Name;

    public TMP_InputField InputField_PositiionX;
    public TMP_InputField InputField_PositiionY;
    public TMP_InputField InputField_PositiionZ;

    public TMP_InputField InputField_RotationX;
    public TMP_InputField InputField_RotationY;
    public TMP_InputField InputField_RotationZ;

    public TMP_InputField InputField_PositiionX_Mod;
    public TMP_InputField InputField_PositiionY_Mod;
    public TMP_InputField InputField_PositiionZ_Mod;

    public TMP_InputField InputField_RotationX_Mod;
    public TMP_InputField InputField_RotationY_Mod;
    public TMP_InputField InputField_RotationZ_Mod;

    // Start is called before the first frame update
    void Start()
    {
        ResetViewer();
    }

    // Update is called once per frame
    void ResetViewer()
    {
        
    }



    public void SetBlock(Transform _transform, string _name = "")
    {
        isReady = false;

        Item = _transform;

        Text_Name.text = _name == "" ? "Transform" : "Transform(" + _name + ")";


        Position = Item.position;
        Rotation = Item.rotation.eulerAngles;

        InputField_PositiionX.text = Position.x.ToString();
        InputField_PositiionY.text = Position.y.ToString();
        InputField_PositiionZ.text = Position.z.ToString();

        InputField_RotationX.text = Rotation.x.ToString();
        InputField_RotationY.text = Rotation.y.ToString();
        InputField_RotationZ.text = Rotation.z.ToString();

        InputField_PositiionX_Mod.text = Position.x.ToString();
        InputField_PositiionY_Mod.text = Position.y.ToString();
        InputField_PositiionZ_Mod.text = Position.z.ToString();

        InputField_RotationX_Mod.text = Rotation.x.ToString();
        InputField_RotationY_Mod.text = Rotation.y.ToString();
        InputField_RotationZ_Mod.text = Rotation.z.ToString();

        isReady = true;
    }


    public void OnValueChange()
    {
        if (isReady)
        {
            Position_New = new Vector3(
                float.Parse(InputField_PositiionX_Mod.text),
                float.Parse(InputField_PositiionY_Mod.text),
                float.Parse(InputField_PositiionZ_Mod.text));

            Rotation_New = new Vector3(
                float.Parse(InputField_RotationX_Mod.text),
                float.Parse(InputField_RotationY_Mod.text),
                float.Parse(InputField_RotationZ_Mod.text));

            // Apply to object

            Item.position = Position_New;
            Item.rotation = Quaternion.Euler(Rotation_New);
        }
    }



    public void OnClick_ResetPositionX()
    {
        Item.position = new Vector3(Position.x, Item.position.y, Item.position.z);
        InputField_PositiionX_Mod.text = Position.x.ToString();
    }

    public void OnClick_ResetPositionY()
    {
        Item.position = new Vector3(Item.position.x, Position.y, Item.position.z);
        InputField_PositiionY_Mod.text = Position.y.ToString();
    }

    public void OnClick_ResetPositionZ()
    {
        Item.position = new Vector3(Item.position.x, Item.position.y, Position.z);
        InputField_PositiionZ_Mod.text = Position.z.ToString();
    }

    public void OnClick_ResetRotationX()
    {
        Item.rotation = Quaternion.Euler(new Vector3(Rotation.x, Item.rotation.eulerAngles.y, Item.rotation.eulerAngles.z));
        InputField_RotationX_Mod.text = transform.rotation.x.ToString();
    }

    public void OnClick_ResetRotationY()
    {
        Item.rotation = Quaternion.Euler(new Vector3(Item.rotation.eulerAngles.x, Rotation.y, Item.rotation.eulerAngles.z));
        InputField_RotationY_Mod.text = transform.rotation.y.ToString();
    }

    public void OnClick_ResetRotationZ()
    {
        Item.rotation = Quaternion.Euler(new Vector3(Item.rotation.eulerAngles.x, Item.rotation.eulerAngles.y, Rotation.z));
        InputField_RotationZ_Mod.text = transform.rotation.z.ToString();
    }
}
