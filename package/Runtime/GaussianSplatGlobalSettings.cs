using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GaussianSplatting.Runtime
{
    [ExecuteAlways]
    public class GaussianSplatGlobalSettings : MonoBehaviour
    {
        public enum SplatRenderStrategy
        {
            PerObject,
            GlobalSort
        }

        public SplatRenderStrategy m_RenderStrategy = SplatRenderStrategy.PerObject;
        public int m_GlobalSortNthFrame = 1;


        public void Awake()
        {
            GaussianSplatRenderSystem.instance.RegisterGlobalSettings(this);
        }

        private void OnEnable()
        {
            GaussianSplatRenderSystem.instance.RegisterGlobalSettings(this);
        }

        private void OnDisable()
        {
            GaussianSplatRenderSystem.instance.UnregisterGlobalSettings();
        }

        private void OnValidate()
        {
            m_GlobalSortNthFrame = Mathf.Max(1, m_GlobalSortNthFrame);
        }

    }
}
