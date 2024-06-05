using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class DissolveAnimation : MonoBehaviour
{
    // The new material to be applied
    public Material newMaterial;
    // The name of the variable in the material to adjust
    public string variableName = "Dissolve";
    // The target value for the variable
    public float targetValue;
    // The duration of the adjustment
    public float duration = 1;
    public GameObject targetGameObject;
    private bool revert = false;

    // Dictionary to store original materials of each renderer
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    // Method to change materials and start the adjustment coroutine
    public void dissolve(bool revert)
    {
        this.revert = revert;
        if (newMaterial == null)
        {
            Debug.LogError("New material not assigned.");
            return;
        }

        // Get all renderers in the target GameObject
        Renderer[] renderers = targetGameObject.GetComponentsInChildren<Renderer>();

        // Loop through all renderers
        foreach (Renderer renderer in renderers)
        {
            // Store the original materials
            originalMaterials[renderer] = renderer.materials;

            // Create an array to hold the new materials
            Material[] newMaterials = new Material[renderer.materials.Length];

            // Replace each material with the new material
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                newMaterials[i] = newMaterial;
            }

            // Assign the new materials to the renderer
            renderer.materials = newMaterials;
        }

        // Start the coroutine to adjust the material variable
        StartCoroutine(AdjustMaterialVariable());

    }

    private IEnumerator AdjustMaterialVariable()
    {
        // Get all renderers in the target GameObject
        Renderer[] renderers = targetGameObject.GetComponentsInChildren<Renderer>();

        // Initial time
        float time = 0;

        // Get the initial value of the variable (assumes all materials have the same initial value)
        float initialValue = 0.4f;

        // Loop over the duration to adjust the variable smoothly
        while (time < duration)
        {
            // Calculate the new value based on time
            float newValue = Mathf.Lerp(initialValue, targetValue, time / duration);
            

            MaterialPropertyBlock finalPropertyBlock = new MaterialPropertyBlock();
            // Set the new value for each material
            foreach (Renderer renderer in renderers)
            {
                newMaterial.SetFloat("Vector1_FEFF47F1", newValue);
                renderer.GetPropertyBlock(finalPropertyBlock);
                finalPropertyBlock.SetFloat("Dissolve", newValue);
                renderer.SetPropertyBlock(finalPropertyBlock);
            }

            // Increment the time
            time += Time.deltaTime;

            // Yield to the next frame
            yield return null;
        }

        // Ensure the final value is set
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.SetFloat(variableName, targetValue);
            }
        }

        if (revert)
        {
            // Revert materials back to original after adjustment
            foreach (Renderer renderer in renderers)
            {
                renderer.materials = originalMaterials[renderer];
            }
        }
    }
}