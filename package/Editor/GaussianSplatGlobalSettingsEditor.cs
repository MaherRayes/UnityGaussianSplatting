// SPDX-License-Identifier: MIT

using GaussianSplatting.Runtime;
using UnityEditor;
using UnityEngine;

namespace GaussianSplatting.Editor
{
    [CustomEditor(typeof(GaussianSplatGlobalSettings))]
    public class GaussianSplatGlobalSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty m_PropRenderStrategy;
        SerializedProperty m_PropGlobalSortNthFrame;
        SerializedProperty m_PropSortMode;
        SerializedProperty m_PropShaderSplats;
        SerializedProperty m_PropShaderComposite;
        SerializedProperty m_PropCSSplatUtilitiesRadix;
        SerializedProperty m_PropCSSplatUtilitiesFFX;

        bool m_ResourcesExpanded = true;

        void OnEnable()
        {
            m_PropRenderStrategy = serializedObject.FindProperty("m_RenderStrategy");
            m_PropGlobalSortNthFrame = serializedObject.FindProperty("m_GlobalSortNthFrame");
            m_PropSortMode = serializedObject.FindProperty("m_SortMode");
            m_PropShaderSplats = serializedObject.FindProperty("m_GlobalSplatsShader");
            m_PropShaderComposite = serializedObject.FindProperty("m_GlobalCompositeShader");
            m_PropCSSplatUtilitiesRadix = serializedObject.FindProperty("m_CSSplatUtilitiesRadix");
            m_PropCSSplatUtilitiesFFX = serializedObject.FindProperty("m_CSSplatUtilitiesFFX");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.Label("Global Render Strategy", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_PropRenderStrategy);

            bool globalSort =
                m_PropRenderStrategy.enumValueIndex ==
                (int)GaussianSplatGlobalSettings.SplatRenderStrategy.GlobalSort;

            using (new EditorGUI.DisabledScope(!globalSort))
            {
                EditorGUILayout.PropertyField(m_PropGlobalSortNthFrame);
            }

            if (globalSort)
            {
                EditorGUILayout.HelpBox(
                    "Global sort mode renders all splats in one globally sorted splat pass. Debug render modes remain per-object only.",
                    MessageType.Info
                );
            }

            EditorGUI.BeginChangeCheck();           
            EditorGUILayout.PropertyField(m_PropSortMode);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                foreach (var obj in targets)
                {
                    if (obj is GaussianSplatRenderer renderer && renderer.enabled)
                    {
                        renderer.Reset();
                        EditorUtility.SetDirty(renderer);
                    }

                    if(obj is GaussianSplatGlobalSettings globalSettings)
                    {
                        globalSettings.ResetGlobalSorter();
                        EditorUtility.SetDirty(globalSettings);
                    }
                }

                return;
            }

            EditorGUILayout.Space();

            m_ResourcesExpanded = EditorGUILayout.Foldout(
                m_ResourcesExpanded,
                "Resources",
                true,
                EditorStyles.foldoutHeader
            );

            if (m_ResourcesExpanded)
            {
                EditorGUILayout.PropertyField(m_PropShaderSplats);
                EditorGUILayout.PropertyField(m_PropShaderComposite);
                EditorGUILayout.PropertyField(m_PropCSSplatUtilitiesRadix);
                EditorGUILayout.PropertyField(m_PropCSSplatUtilitiesFFX);
            }

            if (globalSort && HasMissingResources())
            {
                EditorGUILayout.HelpBox(
                    "Global sort resources are not fully assigned.",
                    MessageType.Error
                );
            }

            serializedObject.ApplyModifiedProperties();
        }

        bool HasMissingResources()
        {
            return m_PropShaderSplats.objectReferenceValue == null ||
                   m_PropShaderComposite.objectReferenceValue == null ||
                   m_PropCSSplatUtilitiesRadix.objectReferenceValue == null ||
                   m_PropCSSplatUtilitiesFFX.objectReferenceValue == null;
        }
    }
}