using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaBIM
{
    public class BIMResourceHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }



        [Header("Color Palette")]
        public List<BIMColor> ColorPalette = new List<BIMColor>();

        public Color GetBIMColor(string _name)
        {
            if (ColorPalette.Find(item => item.Name == _name) != null)
            {
                return ColorPalette.Find(item => item.Name == _name).Color;
            }
            else
            {
                return Color.red;
            }
        }


        [Header("Material")]
        public Material BIM_OBJECT_SELECTION;

        public List<BIMMat> BIM_MATERIALS = new List<BIMMat>();



        public Material GetBIMMat(string _name)
        {
            if (BIM_MATERIALS.Find(item => item.name == _name) != null)
            {
                return BIM_MATERIALS.Find(item => item.name == _name).Material;
            }
            else
            {
                return null;
            }
        }

        [Serializable]
        public class BIMColor
        {
            public string Name = "Theme";
            public Color Color = Color.white;
        }


        [Serializable]
        public class BIMMat
        {
            public string name = "BIM_";
            public Material Material;
        }

    }
 }
