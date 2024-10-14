using UnityEngine;

namespace WorldSpaceTransitions.CrossSection
{
    [ExecuteInEditMode]

    public class EdgeObject : MonoBehaviour
    {
        //public Material maskMaterial;

        public void OnEnable()
        {
            EdgeEffectSystem.instance.Add(gameObject);
        }

        public void Start()
        {
            EdgeEffectSystem.instance.Add(gameObject);
        }

        public void OnDisable()
        {
            EdgeEffectSystem.instance.Remove(gameObject);
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (transform.hasChanged)
            {
                EdgeEffectSystem.instance.Add(gameObject);
            }
        }
#endif
    }
}