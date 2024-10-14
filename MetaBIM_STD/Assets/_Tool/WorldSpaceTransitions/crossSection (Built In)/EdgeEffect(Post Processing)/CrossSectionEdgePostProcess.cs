using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(CrossSectionEdgePostProcessRenderer), PostProcessEvent.BeforeStack, "CrossSection/PerObjectEdgeEffect")]
public sealed class CrossSectionEdgePostProcess : PostProcessEffectSettings
{
    [Tooltip("Number of pixels between samples that are tested for an edge. When this value is 1, tested samples are adjacent.")]
    public IntParameter scale = new IntParameter { value = 1 };

    [Tooltip("Difference between depth values, scaled by the current depth, required to draw an edge.")]
    public FloatParameter depthThreshold = new FloatParameter { value = 1.5f };
    [Range(0, 1), Tooltip("Larger values will require the difference between normals to be greater to draw an edge.")]
    public FloatParameter normalThreshold = new FloatParameter { value = 0.4f };
    [Range(0, 1), Tooltip("The value at which the dot product between the surface normal and the view direction will affect " +
    "the depth threshold. This ensures that surfaces at right angles to the camera require a larger depth threshold to draw " +
    "an edge, avoiding edges being drawn along slopes.")]
    public FloatParameter depthNormalThreshold = new FloatParameter { value = 0.5f };
    [Tooltip("Scale the strength of how much the depthNormalThreshold affects the depth threshold.")]
    public FloatParameter depthNormalThresholdScale = new FloatParameter { value = 7 };
    [Range(0, 10)]
    public FloatParameter maskSensitivity = new FloatParameter { value = 0.0f };
    [Range(0, 10)]
    public FloatParameter colorSensitivity = new FloatParameter { value = 0.0f };
    [Range(0, 10)]
    public FloatParameter backfaceSensitivity = new FloatParameter { value = 0.0f };
    public ColorParameter edgeColor = new ColorParameter { value = Color.white };
    public ColorParameter backfaceEdgeColor = new ColorParameter { value = Color.red };
    public BoolParameter backfacesOnly = new BoolParameter { value = false };
}


public sealed class CrossSectionEdgePostProcessRenderer : PostProcessEffectRenderer<CrossSectionEdgePostProcess>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("CrossSection/EdgePostProcess"));
        sheet.properties.SetFloat("_Scale", settings.scale);
        sheet.properties.SetFloat("_DepthThreshold", settings.depthThreshold);
        sheet.properties.SetFloat("_NormalThreshold", settings.normalThreshold);
        sheet.properties.SetFloat("_DepthNormalThreshold", settings.depthNormalThreshold);
        sheet.properties.SetFloat("_DepthNormalThresholdScale", settings.depthNormalThresholdScale);
        sheet.properties.SetFloat("_MaskSensitivity", settings.maskSensitivity);
        sheet.properties.SetFloat("_ColorSensitivity", settings.colorSensitivity);
        sheet.properties.SetFloat("_BackfaceSensitivity", settings.backfaceSensitivity);

        sheet.properties.SetColor("_edgeColor", settings.edgeColor);
        sheet.properties.SetColor("_backfaceEdgeColor", settings.backfaceEdgeColor);
        sheet.properties.SetFloat("_backfacesOnly", (settings.backfacesOnly? 1f:0f));
        //sheet.properties.SetFloat("_backfacesOnly", settings.backfacesOnly);
        if (!settings.backfacesOnly) sheet.EnableKeyword("ALL_EDGES");
        if (settings.backfacesOnly) sheet.DisableKeyword("ALL_EDGES");

        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;
        sheet.properties.SetMatrix("_ClipToView", clipToView);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}