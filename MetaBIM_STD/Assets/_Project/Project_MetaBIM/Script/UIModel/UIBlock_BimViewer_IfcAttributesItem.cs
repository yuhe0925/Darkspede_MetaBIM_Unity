using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using IfcToolkit;

public class UIBlock_BimViewer_IfcAttributesItem : MonoBehaviour
{
    public RectTransform Layout;
    public TextMeshProUGUI Text_Header;
    public GameObject MarkExpended;
    public GameObject MarkCollapsed;

    public GameObject AttributesItemPrefab;
    public Transform AttributesItemParent;
    public List<GameObject> Attributes;

    public IfcAttributes IfcAttributes;
    public IfcMaterials IfcMaterials;
    public IfcTypes IfcTypes;
    public IfcQuantities IfcQuantities;


    public void Start()
    {
        AttributesItemPrefab.SetActive(false);
    }

    public void SetAttributeBlock(string _title, IfcAttributes _attributes)
    {
        Text_Header.text = _title;
        IfcAttributes = _attributes;

        Attributes = new List<GameObject>();
        AttributesItemPrefab.SetActive(false);

        int index = 0;
        foreach (string name in IfcAttributes.attributes)
        {
            index++;
            GameObject ob = Instantiate(AttributesItemPrefab, AttributesItemParent);
            ob.SetActive(true);
            CompareElement element = Page_BIMCompare.Instance.UpdateAttributes.Find(x => x.ifcid == name);

            if(element != null)
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcAttributes.Find(name), index, element.diff);
            }
            else
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcAttributes.Find(name), index);
            }

            Attributes.Add(ob);
        }
    }

    public void SetPropertyBlock(string _title)
    {
        Text_Header.text = _title;
    }

    public void AddProperty(string _name, string _value, int _index, string _type = "")
    {
        GameObject ob = Instantiate(AttributesItemPrefab, AttributesItemParent);
        ob.SetActive(true);
        ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(_name, _value, _index, _type);
        Attributes.Add(ob);
    }

    public void SetMeterialBlock(string _title, IfcMaterials _meterials)
    {
        Text_Header.text = _title;
        IfcMaterials = _meterials;

        Attributes = new List<GameObject>();
        AttributesItemPrefab.SetActive(false);

        int index = 0;
        foreach (string name in IfcMaterials.materials)
        {
            index++;
            GameObject ob = Instantiate(AttributesItemPrefab, AttributesItemParent);
            ob.SetActive(true);
            CompareElement element = Page_BIMCompare.Instance.UpdateMaterials.Find(x => x.ifcid == name);

            if (element != null)
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcMaterials.Find(name), index, element.diff);
            }
            else
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcMaterials.Find(name), index);
            }

            Attributes.Add(ob);
        }

    }

    public void SetTypeBlock(string _title, IfcTypes _types)
    {
        Text_Header.text = _title;
        IfcTypes = _types;

        Attributes = new List<GameObject>();
        AttributesItemPrefab.SetActive(false);

        int index = 0;
        foreach (string name in IfcTypes.types)
        {
            index++;
            GameObject ob = Instantiate(AttributesItemPrefab, AttributesItemParent);
            ob.SetActive(true);
            CompareElement element = Page_BIMCompare.Instance.UpdateTypes.Find(x => x.ifcid == name);

            if (element != null)
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcTypes.Find(name), index, element.diff);
            }
            else
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcTypes.Find(name), index);
            }

            Attributes.Add(ob);
        }

    }

    public void SetQuantityBlock(string _title, IfcQuantities _quantities)
    {
        Text_Header.text = _title;
        IfcQuantities = _quantities;

        Attributes = new List<GameObject>();
        AttributesItemPrefab.SetActive(false);

        int index = 0;
        foreach (string name in IfcQuantities.quantities)
        {
            index++;
            GameObject ob = Instantiate(AttributesItemPrefab, AttributesItemParent);
            ob.SetActive(true);
            CompareElement element = Page_BIMCompare.Instance.UpdateQuantities.Find(x => x.ifcid == name);

            if (element != null)
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcQuantities.Find(name), index, element.diff);
            }
            else
            {
                ob.GetComponent<UIBlock_BimViewer_IfcAttributesValueItem>().SetBlock(name, IfcQuantities.Find(name), index);
            }

            Attributes.Add(ob);
        }

    }



    public void OnClick_Collapse()
    {
        MarkExpended.SetActive(true);
        MarkCollapsed.SetActive(false);

        foreach (GameObject item in Attributes)
        {
            item.SetActive(false);
        }


        LayoutRebuilder.ForceRebuildLayoutImmediate(Layout);

    }


    public void OnClick_Expend()
    {
        MarkExpended.SetActive(false);
        MarkCollapsed.SetActive(true);

        foreach (GameObject item in Attributes)
        {
            item.SetActive(true);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(Layout);
    }
}
