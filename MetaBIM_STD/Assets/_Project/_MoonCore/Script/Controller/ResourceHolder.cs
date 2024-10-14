using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using MetaBIM;

public class ResourceHolder : MonoBehaviour
{

    public static ResourceHolder Instance;
    public static void GetInstance()
    {
        if (Instance == null)
        {
            Instance = GameObject.Find(Config.Instance_ResourceHolder).GetComponent<ResourceHolder>();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    [Header("Cursor")]
    public Texture2D HoverButtonCursor;
    public Texture2D HoverInputCursor;
    public Texture2D DefaultCursor;
    public Texture2D CopyCursor;
    public Texture2D DragCursor;
    public Texture2D DragHorizontalCursor;
    public Texture2D DragVerticalCursor;
    public Texture2D SizerCursor;


    [Header("Color Palette")]
    public List<ColorItem> ColorPalette;

    [Header("Image and Texture")]
    public Texture2D Image_Loading;
    public Texture2D Image_Error;

    public string Text_ImageLoadingFailed;

    public Color GetThemeColor(string _name)
    {
        if (ColorPalette.Find(item => item.Name == _name) != null){
            return ColorPalette.Find(item => item.Name == _name).Color;
        }
        else
        {
            return Color.red;
        }
    }


    [Header("Runtime Material")]
    public Material BIM_OBJECT_SELECTION;
    public Material BIM_OBJECT_ONHOVER;
    public Material BIM_OBJECT_ISOLATION;
    
    [Header("BIM Objects")]
    public GameObject ModelGameRootObjectTemplate;
    public GameObject ModelGameObjectTemplate;

    public List<ColorItem> MaterialColor;

    [Header("Tool Objects")]
    public List<PrefabItem> PrefabItems;

    public static Color GetRandomColor(float _alpha)
    {
        Color background = new Color(
              Random.Range(0f, 1f),
              Random.Range(0f, 1f),
              Random.Range(0f, 1f),
              _alpha
          ); ;


        return background;
    }

    public ColorItem GetMatItemByName(string _name)
    {
        foreach (ColorItem item in MaterialColor)
        {
            if (item.Name == _name)
            {
                return item;
            }
        }

        return MaterialColor[0];
    }

    public ColorItem SearchMatItem(string _ifcName)
    {
        foreach(ColorItem item in MaterialColor)
        {
            if (_ifcName.Contains(item.Name))
            {
                return item;
            }
        }
        
        return MaterialColor[0];
    }

    public GameObject GetPrefabItem(string _prefabName)
    {
        foreach (var item in PrefabItems)
        {
            if (item.Name == _prefabName)
            {
                return item.Prefab;
            }
        }

        return null;
    }



    [Header("CAD Objects")]
    public List<GameObject> CadEntityPrefabs;



    [Header("Debug References")]
    public GameObject DebugReferencePoiint;
}


[Serializable]
public class ColorItem
{
    public string Name = "Theme";
    public Color Color = Color.white;
    public Material Material;
}


public static class BIM_MATERIAL_NAME
{
    public static string BIM_SOLID = "BIM_SOLID";
    public static string BIM_TRANSPARENT = "BIM_TRANSPARENT";
    public static string BIM_HOVER = "BIM_HOVER";
    public static string BIM_SELECTED = "BIM_SELECED";
    public static string BIM_ISOLATED = "BIM_ISOLATED";
    
    
}

[Serializable]
public class PrefabItem
{
    public string Name = "default";
    public GameObject Prefab;
}

