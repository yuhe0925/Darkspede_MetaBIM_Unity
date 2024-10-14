using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
namespace WorldSpaceTransitions.CrossSection
{
    //[RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class EdgeEffectSystem
    {
        static EdgeEffectSystem m_Instance; // singleton
        static public EdgeEffectSystem instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new EdgeEffectSystem();
                return m_Instance;
            }
        }

        internal HashSet<GameObject> m_EdgeObjs = new HashSet<GameObject>();

        public void Add(GameObject o)
        {
            Remove(o);
            m_EdgeObjs.Add(o);
            Debug.Log("added effect " + o.gameObject.name);
            if (EdgeRenderer.instance)
            {
                EdgeRenderer.instance.OnEnable();
            }
        }

        public void Remove(GameObject o)
        {
            m_EdgeObjs.Remove(o);
            Debug.Log("removed effect " + o.gameObject.name);
            if (EdgeRenderer.instance)
            {
                EdgeRenderer.instance.OnEnable();
            }
        }
    }


    [ExecuteInEditMode]
    public class EdgeRenderer : MonoBehaviour
    {
        public Material maskMaterial;
        public Shader edgeEffectShader;//this reference is to force the shader to get into the build
        private CommandBuffer m_EdgeBuffer;
        private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();

        //static EdgeRenderer m_Instance; // singleton
        static public EdgeRenderer instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }

            instance = this;
            //if (Application.isPlaying) CombineMeshes();//command buffers are not statically batched
            //to do: some static batching optimisation
        }

        private void Cleanup()
        {
            foreach (var cam in m_Cameras)
            {
                if (cam.Key)
                    //cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
                    cam.Key.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, cam.Value);
            }
            m_Cameras.Clear();
        }

        public void OnDisable()
        {
            Cleanup();
        }

        public void OnEnable()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }

            instance = this;

            Cleanup();
        }

        public void OnWillRenderObject()
        {
            var render = gameObject.activeInHierarchy && enabled;
            if (!render)
            {
                Cleanup();
                return;
            }

            var cam = Camera.current;
            if (!cam)
                return;

            if (m_Cameras.ContainsKey(cam))
                return;

            // create new command buffer
            m_EdgeBuffer = new CommandBuffer();
            m_EdgeBuffer.name = "Edge map buffer";
            m_Cameras[cam] = m_EdgeBuffer;

            var edgeEffectSystem = EdgeEffectSystem.instance;

            // create render texture for glow map
            int tempID = Shader.PropertyToID("_Temp1");
            m_EdgeBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Point);
            m_EdgeBuffer.SetRenderTarget(tempID);
            m_EdgeBuffer.ClearRenderTarget(true, true, Color.black); // clear before drawing to it each frame!!

            // draw all glow objects to it
            Debug.Log(edgeEffectSystem.m_EdgeObjs.Count.ToString());
            foreach (GameObject o in edgeEffectSystem.m_EdgeObjs)
            {
                Renderer [] r = o.GetComponentsInChildren<Renderer>();        
                if (maskMaterial)
                {
                    foreach (Renderer rend in r)
                    {
                        //if (rend.gameObject.isStatic && Application.isPlaying) continue;
                        m_EdgeBuffer.DrawRenderer(rend, maskMaterial);
                    }
                }
            }
            Renderer renderer = GetComponent<Renderer>();
            if (renderer&&maskMaterial) m_EdgeBuffer.DrawRenderer(renderer, maskMaterial);
            //MeshFilter mf = GetComponent<MeshFilter>();
            //Matrix4x4 M = transform.localToWorldMatrix;
            //if (maskMaterial && mf) m_EdgeBuffer.DrawMesh(mf.sharedMesh, M, maskMaterial); 
            // set render texture as globally accessable texture
            m_EdgeBuffer.SetGlobalTexture("_EdgeMap", tempID);
            //cam.AddCommandBuffer(CameraEvent.BeforeLighting, m_EdgeBuffer);//Deferred
            cam.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, m_EdgeBuffer);
        }

        public void CombineMeshes() //to do: some static batching optimisation
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (!mf) gameObject.AddComponent<MeshFilter>();
            //Renderer rend = GetComponent<Renderer>();
            //if (!rend) rend = gameObject.AddComponent<MeshRenderer>();
            EdgeObject[] edgeobj = (EdgeObject[])FindObjectsOfType(typeof(EdgeObject));
            var edgeEffectSystem = EdgeEffectSystem.instance;
            List<MeshFilter> filterList = new List<MeshFilter>();
            foreach (EdgeObject o in edgeobj)
            {
                Renderer[] r = o.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in r)
                {
                    if (renderer.gameObject.isStatic)
                    {
                        Debug.Log(renderer.name);
                        MeshFilter mfilter = renderer.GetComponent<MeshFilter>();
                        if(mfilter) filterList.Add(mfilter);
                    }
                }
            }
            MeshFilter[] meshFilters = filterList.ToArray();

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            //rend.enabled = false;
        }
    }
}