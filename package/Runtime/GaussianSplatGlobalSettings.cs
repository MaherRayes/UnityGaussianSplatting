using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

        public enum SortMode
        {
            Radix,
            FFX
        }

        public SplatRenderStrategy m_RenderStrategy = SplatRenderStrategy.PerObject;

        [Min(1)]
        public int m_GlobalSortNthFrame = 1;

        public SortMode m_SortMode;

        [Header("Global Sort Materials")]
        [SerializeField] Shader m_GlobalSplatsShader;
        [SerializeField] Shader m_GlobalCompositeShader;
        [SerializeField] ComputeShader m_CSSplatUtilitiesRadix;
        [SerializeField] ComputeShader m_CSSplatUtilitiesFFX;

        Material m_GlobalSplatsMaterial;
        Material m_GlobalCompositeMaterial;

        public Material GlobalSplatsMaterial => m_GlobalSplatsMaterial;
        public Material GlobalCompositeMaterial => m_GlobalCompositeMaterial;
        public ComputeShader CSSplatUtilitiesRadix => m_CSSplatUtilitiesRadix;
        public ComputeShader CSSplatUtilitiesFFX => m_CSSplatUtilitiesFFX;

        public bool EnsureMaterials()
        {
            if (m_GlobalSplatsShader == null || m_GlobalCompositeShader == null || m_CSSplatUtilitiesRadix == null || m_CSSplatUtilitiesFFX == null)
                return false;

            if (m_GlobalSplatsMaterial == null)
                m_GlobalSplatsMaterial = new Material(m_GlobalSplatsShader)
                {
                    name = "Global Gaussian Splats"
                };

            if (m_GlobalCompositeMaterial == null)
                m_GlobalCompositeMaterial = new Material(m_GlobalCompositeShader)
                {
                    name = "Global Gaussian Composite"
                };

            return true;
        }

        private void OnEnable()
        {
            GaussianSplatRenderSystem.instance.RegisterGlobalSettings(this);
        }

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                Destroy(m_GlobalSplatsMaterial);
                Destroy(m_GlobalCompositeMaterial);
            }
            else
            {
                DestroyImmediate(m_GlobalSplatsMaterial);
                DestroyImmediate(m_GlobalCompositeMaterial);
            }

            m_GlobalSplatsMaterial = null;
            m_GlobalCompositeMaterial = null;

            GaussianSplatRenderSystem.instance.UnregisterGlobalSettings(this);
        }

        private void OnValidate()
        {
            m_GlobalSortNthFrame = Mathf.Max(1, m_GlobalSortNthFrame);
        }

        public void ResetGlobalSorter()
        {
            GaussianSplatRenderSystem.instance.m_GlobalRenderer.ResetSorter();
        }

        //public static SortMode ResolveSortMode(SortMode requested) => isRadixSupported ? requested : SortMode.FFX;

    }
}
