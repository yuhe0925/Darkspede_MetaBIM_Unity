using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;

namespace WorldSpaceTransitions.Standard
{
    [ExecuteInEditMode]
    public class EffectToggles : MonoBehaviour
    {
        public PostProcessProfile v2profile;
        private bool kwdOn = true;
        private bool edgeAterStack = false;

        void Start()
        {
            if (!v2profile) return;
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcess ef))
                kwdOn = ef.backfacesOnly;
        }
        void OnEnable()
        {
            if (!v2profile) return;
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcess ef))
                kwdOn = ef.backfacesOnly;
        }

        void OnDisable()
        {
            if (!v2profile) return;
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcess ef)) ef.backfacesOnly.Override(kwdOn);
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcessAfterStack ef_as))
            {
                ef_as.backfacesOnly.Override(kwdOn);
                ef_as.enabled.Override(false);
            }
            if (v2profile.TryGetSettings(out AmbientOcclusion ao)) ao.enabled.Override(false);
            if (v2profile.TryGetSettings(out ChromaticAberration ca)) ca.enabled.Override(false);
            if (v2profile.TryGetSettings(out LensDistortion ld)) ld.enabled.Override(false);
            if (v2profile.TryGetSettings(out Bloom bl)) bl.enabled.Override(false);
        }

        public void ShowEdges(bool val)
        {
            //if (edgeEff) edgeEff.enabled = new BoolParameter { value = val };
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcess ef)) ef.enabled.Override(!edgeAterStack&&val);
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcessAfterStack ef_as)) ef_as.enabled.Override(edgeAterStack && val);
         }

        public void BackfaceEdgesOnly(bool val)
        {
            //if (edgeEff) edgeEff.backfacesOnly = new BoolParameter { value = val };
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcess ef)) ef.backfacesOnly.Override(val);
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcessAfterStack ef_as)) ef_as.backfacesOnly.Override(val);
        }
        public void SetAO (bool val)
        {
            if(v2profile.TryGetSettings(out AmbientOcclusion ao)) ao.enabled.Override(val);
        }
        public void SetCA(bool val)
        {
            if (v2profile.TryGetSettings(out ChromaticAberration ca)) ca.enabled.Override(val);
        }
        public void SetLD(bool val)
        {
            if (v2profile.TryGetSettings(out LensDistortion ld)) ld.enabled.Override(val);
        }
        public void SetBL(bool val)
        {
            if (v2profile.TryGetSettings(out Bloom bl)) bl.enabled.Override(val);
        }
        public void SetEdgeAfterStack(bool val)
        {
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcess ef)) ef.enabled.Override(!val);
            if (v2profile.TryGetSettings(out CrossSectionEdgePostProcessAfterStack ef_as)) ef_as.enabled.Override(val);
            edgeAterStack = val;
        }

    }
}