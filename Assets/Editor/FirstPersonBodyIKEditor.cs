#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// IMPORTANT: this file must live in a folder named "Editor" anywhere under Assets
// (e.g. Assets/Scripts/Editor/FirstPersonBodyIKEditor.cs) so Unity excludes it from builds.

namespace FirstPersonIK.EditorTools
{
    [CustomEditor(typeof(FirstPersonBodyIK))]
    public class FirstPersonBodyIKEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var comp = (FirstPersonBodyIK)target;

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Assign Camera Transform and Driven Bone above (Hips too, if you want the XZ " +
                "anti-clip lock). If this is a Humanoid Animator you can auto-fill Hips/Chest instead.",
                MessageType.Info);

            if (GUILayout.Button("Auto-Assign Hips/Chest From Humanoid Rig", GUILayout.Height(28)))
            {
                AutoAssign(comp);
            }
        }

        static void AutoAssign(FirstPersonBodyIK comp)
        {
            var animator = comp.animator != null ? comp.animator : comp.GetComponent<Animator>();
            if (animator == null || !animator.isHuman)
            {
                EditorUtility.DisplayDialog("Auto-Assign",
                    "This only works with a Humanoid Animator. Assign Hips and Driven Bone manually instead.",
                    "OK");
                return;
            }

            Undo.RecordObject(comp, "Auto-Assign First Person Body IK");
            comp.animator = animator;
            comp.hips = animator.GetBoneTransform(HumanBodyBones.Hips);

            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            comp.drivenBone = chest != null ? chest : animator.GetBoneTransform(HumanBodyBones.Spine);

            EditorUtility.SetDirty(comp);

            EditorUtility.DisplayDialog("Auto-Assign",
                "Hips and Driven Bone assigned from the Humanoid Avatar.\n\n" +
                "Double-check Local Forward/Up Axes still match your actual bone orientation " +
                "before testing.", "OK");
        }
    }
}
#endif
