using UnityEngine;

public class BitLight : MonoBehaviour
{
    public Light unityLight; // Unity light component
    public Renderer emissiveMesh; // Renderer for the emissive mesh
    public Color emissiveColor = Color.white; // Color of the emissive mesh when lit
    public bool isInitiallyOn = false; // Manual override to set the initial state

    private Material emissiveMaterial;
    private Color originalEmissiveColor;

    void Start()
    {
        if (emissiveMesh != null)
        {
            emissiveMaterial = emissiveMesh.material;
            originalEmissiveColor = emissiveMaterial.GetColor("_EmissionColor");
        }

        if (isInitiallyOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }

    public void TurnOn()
    {
        if (unityLight != null)
        {
            unityLight.enabled = true;
            unityLight.color = emissiveColor;
        }

        if (emissiveMaterial != null)
        {
            emissiveMaterial.SetColor("_EmissionColor", emissiveColor);
            DynamicGI.SetEmissive(emissiveMesh, emissiveColor);
        }
    }

    public void TurnOff()
    {
        if (unityLight != null)
        {
            unityLight.enabled = false;
        }

        if (emissiveMaterial != null)
        {
            emissiveMaterial.SetColor("_EmissionColor", originalEmissiveColor);
            DynamicGI.SetEmissive(emissiveMesh, Color.black);
        }
    }

    public void SetColor(Color newColor)
    {
        emissiveColor = newColor;

        if (unityLight != null)
        {
            unityLight.color = emissiveColor;
        }

        if (emissiveMaterial != null)
        {
            emissiveMaterial.SetColor("_EmissionColor", emissiveColor);
            DynamicGI.SetEmissive(emissiveMesh, emissiveColor);
        }
    }
}
