using UnityEngine;

namespace FirstPersonIK
{
    /// <summary>
    /// First-person body IK, done with plain Transform math instead of Animation Rigging
    /// constraints. This exists because Animation Rigging's Multi*Constraint components kept
    /// producing broken/twisted results for this rig (likely due to axis-masking/offset math
    /// interacting badly with this skeleton's unusual bone orientation), and that math is not
    /// something that can be reliably guessed without testing directly in the Editor.
    ///
    /// This script instead:
    /// 1) Rotates ONE bone (drivenBone, e.g. Chest) directly, in LateUpdate, so its own local
    ///    forward/up axes point toward the direction the camera is looking. Only one bone is
    ///    touched directly - everything below/above it in the hierarchy follows through the
    ///    skin as normal, so there is no compounding rotation across a bone chain.
    /// 2) Locks that same bone's world XZ position to the hips afterward, so the rotation in
    ///    step 1 can't visually slide/clip the mesh sideways.
    ///
    /// Because this runs in LateUpdate - after the Animator has applied the current frame's
    /// animation pose - it always has the final say over drivenBone's rotation/position for
    /// that frame.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class FirstPersonBodyIK : MonoBehaviour
    {
        [Header("References")]
        public Animator animator;
        [Tooltip("The first-person camera whose pitch the torso should follow.")]
        public Transform cameraTransform;
        [Tooltip("The hips/pelvis bone. drivenBone's XZ position gets locked to this every frame.")]
        public Transform hips;
        [Tooltip("The single bone that gets rotated to face the camera (e.g. Chest / UpperChest). " +
                 "Only this one bone is driven directly.")]
        public Transform drivenBone;

        [Header("Driven Bone's Local Axes")]
        [Tooltip("Which local axis of drivenBone points 'forward' out of the character's chest.")]
        public Vector3 localForwardAxis = new Vector3(0f, -1f, 0f);
        [Tooltip("Which local axis of drivenBone points 'up'.")]
        public Vector3 localUpAxis = new Vector3(-1f, 0f, 0f);

        [Header("Pitch Clamp (degrees)")]
        public float minPitch = -70f;
        public float maxPitch = 80f;

        [Header("Behaviour")]
        [Tooltip("Follow camera yaw too. Usually OFF - body yaw is already handled by root motion / turning, so this just controls up/down look-bend.")]
        public bool followYaw = false;
        [Range(0f, 1f)]
        [Tooltip("How strongly to pull the bone toward facing the camera vs. leaving its animated rotation alone. 1 = fully face camera every frame.")]
        public float rotationBlend = 0.85f;
        [Tooltip("Lock drivenBone's XZ world position to the hips, so rotating it can't visually slide/clip the mesh.")]
        public bool lockXZToHips = true;

        void Reset()
        {
            animator = GetComponent<Animator>();
        }

        void LateUpdate()
        {
            if (cameraTransform == null || drivenBone == null)
                return;

            // --- Rotation: point drivenBone's own local forward/up axes at the camera's look direction ---
            Vector3 camEuler = cameraTransform.eulerAngles;
            float pitch = Mathf.Clamp(NormalizeAngle(camEuler.x), minPitch, maxPitch);
            float yaw = followYaw ? camEuler.y : transform.eulerAngles.y;

            Quaternion desiredWorldOrientation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredWorldForward = desiredWorldOrientation * Vector3.forward;
            Vector3 desiredWorldUp = desiredWorldOrientation * Vector3.up;

            // localBasis: rotation that turns the "standard" forward/up (Z/Y) into this bone's
            // own local forward/up axes. worldBasis: rotation that turns standard forward/up
            // into the direction we actually want the bone to face in the world. Removing
            // localBasis and applying worldBasis instead gives the bone rotation that makes its
            // OWN forward/up axes point the way we want, no matter what those axes are.
            Quaternion localBasis = Quaternion.LookRotation(localForwardAxis.normalized, localUpAxis.normalized);
            Quaternion worldBasis = Quaternion.LookRotation(desiredWorldForward, desiredWorldUp);
            Quaternion desiredBoneRotation = worldBasis * Quaternion.Inverse(localBasis);

            drivenBone.rotation = Quaternion.Slerp(drivenBone.rotation, desiredBoneRotation, rotationBlend);

            // --- Position: pin XZ to the hips so the rotation above can't slide the mesh ---
            if (lockXZToHips && hips != null)
            {
                Vector3 pos = drivenBone.position;
                pos.x = hips.position.x;
                pos.z = hips.position.z;
                drivenBone.position = pos;
            }
        }

        static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            return angle;
        }
    }
}
