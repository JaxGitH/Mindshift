using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class DemoSceneManager : MonoBehaviour
    {
        [Header("Character")]

        [SerializeField]
        CharacterActor characterActor = null;


        [Header("Scene references")]

        [SerializeField]
        CharacterReferenceObject[] references = null;

        [Header("UI")]

        [SerializeField]
        Canvas infoCanvas = null;

        [SerializeField]
        bool hideAndConfineCursor = true;

        [Header("Graphics")]


        [SerializeField]
        GameObject graphicsObject = null;

        [Header("Camera")]

        [SerializeField]
        new Camera3D camera = null;

        [UnityEngine.Serialization.FormerlySerializedAs("frameRateText")]
        [SerializeField]
        UnityEngine.UI.Text targetFrameRateText = null;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        Renderer[] graphicsRenderers = null;
        Renderer[] capsuleRenderers = null;

        NormalMovement normalMovement = null;

        float GetRefreshRateValue()
        {
#if UNITY_2023_1_OR_NEWER
            return (float)Screen.currentResolution.refreshRateRatio.value;
#else
            return Screen.currentResolution.refreshRate;
#endif
        }

        void Awake()
        {
            if (characterActor != null)
                normalMovement = characterActor.GetComponentInChildren<NormalMovement>();

            // Set the looking direction mode
            if (normalMovement != null && camera != null)
            {
                if (camera.cameraMode == Camera3D.CameraMode.FirstPerson)
                    normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.ExternalReference;
                else
                    normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.Movement;
            }


            if (graphicsObject != null)
                graphicsRenderers = graphicsObject.GetComponentsInChildren<Renderer>(true);

            Cursor.visible = !hideAndConfineCursor;
            Cursor.lockState = hideAndConfineCursor ? CursorLockMode.Locked : CursorLockMode.None;

            if (targetFrameRateText != null)
            {
                targetFrameRateText.fontSize = 15;
                targetFrameRateText.rectTransform.sizeDelta = new Vector2(
                    300f,
                    40f
                );

                if (QualitySettings.vSyncCount == 1)
                {
                    targetFrameRateText.text = "Target frame rate = " + (GetRefreshRateValue()) + " fps ( Full Vsync )";
                }
                else if (QualitySettings.vSyncCount == 2)
                {
                    targetFrameRateText.text = "Target frame rate = " + (GetRefreshRateValue() / 2) + " fps ( Half Vsync )";
                }
                else if (QualitySettings.vSyncCount == 0)
                {
                    if (Application.targetFrameRate == -1)
                        targetFrameRateText.text = $"Target frame rate = Unlimited";
                    else
                        targetFrameRateText.text = $"Target frame rate = { Application.targetFrameRate } fps";
                }

            }
        }

        void Update()
        {

            for (int index = 0; index < references.Length; index++)
            {
                if (references[index] == null)
                    break;

                if (Input.GetKeyDown(KeyCode.Alpha1 + index) || Input.GetKeyDown(KeyCode.Keypad1 + index))
                {
                    GoTo(references[index]);
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (infoCanvas != null)
                    infoCanvas.enabled = !infoCanvas.enabled;
            }


            if (Input.GetKeyDown(KeyCode.V))
            {
                // If the Camera3D is present, change between First person and Third person mode.
                if (camera != null)
                {
                    camera.ToggleCameraMode();

                    if (normalMovement != null)
                    {
                        if (camera.cameraMode == Camera3D.CameraMode.FirstPerson)
                            normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.ExternalReference;
                        else
                            normalMovement.lookingDirectionParameters.lookingDirectionMode = LookingDirectionParameters.LookingDirectionMode.Movement;

                    }
                }
            }

        }



        void HandleVisualObjects(bool showCapsule)
        {
            if (capsuleRenderers != null)
                for (int i = 0; i < capsuleRenderers.Length; i++)
                    capsuleRenderers[i].enabled = showCapsule;

            if (graphicsRenderers != null)
                for (int i = 0; i < graphicsRenderers.Length; i++)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)graphicsRenderers[i];
                    if (skinnedMeshRenderer != null)
                        skinnedMeshRenderer.forceRenderingOff = showCapsule;
                    else
                        graphicsRenderers[i].enabled = !showCapsule;
                }


        }

        void GoTo(CharacterReferenceObject reference)
        {
            if (reference == null)
                return;

            if (characterActor == null)
                return;

            characterActor.constraintUpDirection = reference.referenceTransform.up;
            characterActor.Teleport(reference.referenceTransform);

            characterActor.upDirectionReference = reference.verticalAlignmentReference;
            characterActor.upDirectionReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;

        }
    }

}
