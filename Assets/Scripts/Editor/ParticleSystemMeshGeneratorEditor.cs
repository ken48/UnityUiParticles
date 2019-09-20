using UnityEngine;
using UnityEditor;

namespace UnityUiParticles
{
    [CustomEditor(typeof(ParticleSystemMeshGenerator), false)]
    [CanEditMultipleObjects]
    public class ParticleSystemMeshGeneratorEditor : Editor
    {
        SerializedProperty _material;
        SerializedProperty _trailsMaterial;
        ParticleSystemRenderer _particleSystemRenderer;
        ParticleSystem.MainModule _mainModule;
        ParticleSystem.TextureSheetAnimationModule _texSheetAnimationModule;

        void OnEnable()
        {
            _material = serializedObject.FindProperty("_material");
            _trailsMaterial = serializedObject.FindProperty("_trailsMaterial");

            _particleSystemRenderer = ((ParticleSystemMeshGenerator)target).GetComponent<ParticleSystemRenderer>();

            var particleSystem = ((ParticleSystemMeshGenerator)target).GetComponent<ParticleSystem>();
            _mainModule = particleSystem.main;
            _texSheetAnimationModule = particleSystem.textureSheetAnimation;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_material);
            EditorGUILayout.PropertyField(_trailsMaterial);
            serializedObject.ApplyModifiedProperties();

            if (_particleSystemRenderer.enabled)
                EditorGUILayout.HelpBox("ParticleSystemRenderer has to be disabled", MessageType.Error);

            // Using Trails module leads to using 2 materials with 2 different textures determined inside each material.
            // Sprites mode in Texture sheet animation module requires CanvasRenderer.SetTexture that overrides texture for all materials.
            // Also the requirement of the 'Sprites' mode that all the sprites were inside the same texture, makes it redundant.
            // Just use the 'Grid' mode instead.
            if (_texSheetAnimationModule.enabled && _texSheetAnimationModule.mode == ParticleSystemAnimationMode.Sprites)
                EditorGUILayout.HelpBox("Texture sheet animation 'Sprites' mode is unsupported", MessageType.Error);

            if (_mainModule.simulationSpace == ParticleSystemSimulationSpace.World &&
                _mainModule.scalingMode != ParticleSystemScalingMode.Hierarchy)
            {
                EditorGUILayout.HelpBox("Set Hierarchy scaling mode for World simulation space", MessageType.Error);
            }
        }
    }
}
