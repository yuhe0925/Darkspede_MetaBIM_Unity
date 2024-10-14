using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace WorldSpaceTransitions.CrossSection
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]

    public class CameraDepthNormalsSettings : MonoBehaviour
    {
        public Shader customShader;
        public DepthTextureMode _depthTextureMode;
        // Use this for initialization
        void Start()
        {
            GetComponent<Camera>().depthTextureMode = _depthTextureMode;
            if (customShader != null)
            {
                GraphicsSettings.SetShaderMode(BuiltinShaderType.DepthNormals, BuiltinShaderMode.UseCustom);
                GraphicsSettings.SetCustomShader(BuiltinShaderType.DepthNormals, customShader);
            }
            else
            {
                GraphicsSettings.SetShaderMode(BuiltinShaderType.DepthNormals, BuiltinShaderMode.UseBuiltin);
            }
        }

        void OnEnable()
        {
            GetComponent<Camera>().depthTextureMode = _depthTextureMode;
            if (customShader != null)
            {
                GraphicsSettings.SetShaderMode(BuiltinShaderType.DepthNormals, BuiltinShaderMode.UseCustom);
                GraphicsSettings.SetCustomShader(BuiltinShaderType.DepthNormals, customShader);
            }
            else
            {
                GraphicsSettings.SetShaderMode(BuiltinShaderType.DepthNormals, BuiltinShaderMode.UseBuiltin);
            }
        }
        void OnDisable()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
            //GraphicsSettings.SetCustomShader(BuiltinShaderType.DepthNormals, customShader);
        }

    }
}