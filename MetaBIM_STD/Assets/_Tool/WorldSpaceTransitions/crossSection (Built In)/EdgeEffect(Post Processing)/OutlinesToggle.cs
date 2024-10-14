using UnityEngine;
using UnityEngine.Rendering;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;

namespace WorldSpaceTransitions.Standard
{
    //[ExecuteInEditMode]
    public class OutlinesToggle : MonoBehaviour
    {
        private bool kwdOn = true;
        private CrossSectionEdgePostProcess edgeEff = null;


        void Start()
        {
            edgeEff = FindObjectOfType<CrossSectionEdgePostProcess>();
            if (edgeEff) kwdOn = edgeEff.backfacesOnly;
        }
        void OnEnable()
        {
            edgeEff = FindObjectOfType<CrossSectionEdgePostProcess>();
            if (edgeEff) kwdOn = edgeEff.backfacesOnly;
        }

        void OnDisable()
        {
            if (edgeEff) edgeEff.backfacesOnly = new BoolParameter { value = kwdOn };
        }

        public void ShowEdges(bool val)
        {
            if (edgeEff) edgeEff.enabled = new BoolParameter { value = val };
        }

        public void BackfaceEdgesOnly(bool val)
        {
            if (edgeEff) edgeEff.backfacesOnly = new BoolParameter { value = val };
        }
    }
}