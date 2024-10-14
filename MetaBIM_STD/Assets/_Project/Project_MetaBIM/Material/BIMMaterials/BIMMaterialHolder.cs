using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MetaBIM
{
    public class BIMMaterialHolder : MonoBehaviour
    {
        public static BIMMaterialHolder Instance;

        public static BIMMaterialHolder GetInstance()
        {
            if (Instance == null)
            {
                Instance = GameObject.Find(Config.Instance_BIMMaterialHolder).GetComponent<BIMMaterialHolder>();
            }

            return Instance;
        }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public string ShaderName = "CrossSection/Standard";
        [Header("RunTime Material")]
        public List<MaterialItem> RuntimeMaterialItems;
        public List<String> MaterialTransparentList;



        public MaterialItem GetMaterialItem(string _name)
        {
            foreach (var item in RuntimeMaterialItems)
            {
                if (item.Name == _name)
                {
                    return item;
                }
            }

            return null;
        }


        public Material ConvertMaterials(string _ifcMaterialName, Color _color)
        {
            MaterialItem mat = GetMaterialItem(_ifcMaterialName);

            if (mat == null)
            {

                mat = new MaterialItem();
                mat.Name = _ifcMaterialName;

                // create new Material
                Material material = new Material(Shader.Find(ShaderName));
                material.name = _ifcMaterialName;


                bool isTransparent = false;

                foreach (var item in MaterialTransparentList)
                {
                    if (_ifcMaterialName.ToLower().Contains(item))
                    {
                        isTransparent = true;
                    }
                }

                if (isTransparent)
                {
                    //_color.a = 0.4f;
                }

                material.color = _color;

                // set material color

                mat.Material = material;
                mat.Color = _color;

                RuntimeMaterialItems.Add(mat);
            }

            return mat.Material;
        }



    }


    [Serializable]
    public class MaterialItem
    {
        public string Name = "Theme";
        public Color Color = Color.white;
        public Material Material;
    }
}

