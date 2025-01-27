using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.ProBuilder;
[InitializeOnLoad]
public class ProBuilderMaterialEditor
{
    // A HashSet to track processed objects to avoid re-processing
    private static HashSet<GameObject> processedObjects = new HashSet<GameObject>();

    // Static constructor: This will run automatically when Unity starts
    static ProBuilderMaterialEditor()
    {
        // Subscribe to Unity's hierarchyChanged event to detect when objects are added
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        // Find all ProBuilder objects in the current scene
        ProBuilderMesh[] allProBuilderObjects = GameObject.FindObjectsOfType<ProBuilderMesh>();

        foreach (ProBuilderMesh pbObj in allProBuilderObjects)
        {
            GameObject obj = pbObj.gameObject;

            // Skip objects that have already been processed
            if (processedObjects.Contains(obj))
                continue;

            // Check if the object already has the MaterialInstance script
            if (obj.GetComponent<MaterialInstance>() == null)
            {
                // If not, add the script
                obj.AddComponent<MaterialInstance>();
            }

            // Mark this object as processed
            processedObjects.Add(obj);
        }
    }
}

