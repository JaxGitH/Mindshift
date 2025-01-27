using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class MaterialInstance : MonoBehaviour
{
    [SerializeField] private Color objectColor = Color.white;

    private Renderer objectRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void OnValidate()
    {
        // Get the Renderer component
        if (objectRenderer == null)
            objectRenderer = GetComponent<Renderer>();

        // Initialize the MaterialPropertyBlock
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        // Set the per-object color
        propertyBlock.SetColor("_Color", objectColor);

        // Apply the PropertyBlock to the Renderer
        objectRenderer.SetPropertyBlock(propertyBlock);
    }
}

