using UnityEngine;

public class GalaxySkyboxCreator : MonoBehaviour
{
    public Material generatedSkybox;

    void Awake()
    {
        if (RenderSettings.skybox == null)
        {
            generatedSkybox = new Material(Shader.Find("Skybox/Procedural"));
            // Set some star/galaxy-like parameters
            generatedSkybox.SetColor("_SkyTint", new Color(0.1f, 0.1f, 0.2f));
            generatedSkybox.SetFloat("_Exposure", 1.2f);
            generatedSkybox.SetFloat("_AtmosphereThickness", 0.4f);
            generatedSkybox.SetColor("_GroundColor", Color.black);
            RenderSettings.skybox = generatedSkybox;
        }
    }
} 