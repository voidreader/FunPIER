using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LegacyAllIn1SpriteShader : MaterialEditorInspector
{
    protected override void CreateToggleList()
    {
        Dictionary<string, ShaderPropertyAtributes> glowToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Glow Texture used?", new ShaderPropertyAtributes("GLOWTEX_ON", "_GlowTexUsed") },
        };
        Toggles.Add(new FeatureToggle("2.Glow (needs BLOOM in scene)", "Glow", "GLOW_ON", "GLOW_OFF", glowToggles));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Fade and Burn", "Fade", "FADE_ON", "FADE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> outBaseToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Outline Base High Resolution? (8 samples instead of 4)", new ShaderPropertyAtributes("OUTBASE8DIR_ON", "_Outline8Directions") },
            { "Outline Base is Pixel Perfect?", new ShaderPropertyAtributes("OUTBASEPIXELPERF_ON", "_OutlineIsPixel") },
        };
        Toggles.Add(new FeatureToggle("3.Outline Base", "Outline Base", "OUTBASE_ON", "OUTBASE_OFF", outBaseToggles));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> outTexToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Outline Texture is Greyscaled?", new ShaderPropertyAtributes("OUTGREYTEXTURE_ON", "_OutlineTexGrey") },
        };
        Toggles.Add(new FeatureToggle("4.Outline Texture (needs Outline Base enabled)", "Outline Texture", "OUTTEX_ON", "OUTTEX_OFF", outTexToggles));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("5.Outline Distortion (needs Outline Base enabled)", "Outline Distortion", "OUTDIST_ON", "OUTDIST_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Gradient Overlay", "Gradient", "GRADIENT_ON", "GRADIENT_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("7.Color Swap", "Color Swap", "COLORSWAP_ON", "COLORSWAP_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Hue Shift (HSV)", "Hue Shift", "HSV_ON", "HSV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("8.Hit Effect", "Hit Effect", "HITEFFECT_ON", "HITEFFECT_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("9.Negative", "Negative", "NEGATIVE_ON", "NEGATIVE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("10.Pixelate", "Pixelate", "PIXELATE_ON", "PIXELATE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> colorRampToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Color ramp affects outline and glow?", new ShaderPropertyAtributes("COLORRAMPOUTLINE_ON", "_ColorRampOutline") },
        };
        Toggles.Add(new FeatureToggle("11.Color ramp", "Color ramp", "COLORRAMP_ON", "COLORRAMP_OFF", colorRampToggles));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> greyScaleToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Greyscale affects outline and glow?", new ShaderPropertyAtributes("GREYSCALEOUTLINE_ON", "_GreyscaleOutline") },
        };
        Toggles.Add(new FeatureToggle("12.Greyscale", "Greyscale", "GREYSCALE_ON", "GREYSCALE_OFF", greyScaleToggles));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> posterizeToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Posterize affects outline and glow?", new ShaderPropertyAtributes("POSTERIZEOUTLINE_ON", "_PosterizeOutline") },
        };
        Toggles.Add(new FeatureToggle("13.Posterize", "Posterize", "POSTERIZE_ON", "POSTERIZE_OFF", posterizeToggles));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> blurToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Blur is HD? (Performance intensive)", new ShaderPropertyAtributes("BLURISHD_ON", "_BlurHD") },
        };
        Toggles.Add(new FeatureToggle("14.Blur", "Blur", "BLUR_ON", "BLUR_OFF", blurToggles));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Motion Blur", "Motion Blur", "MOTIONBLUR_ON", "MOTIONBLUR_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("15.Ghost", "Ghost", "GHOST_ON", "GHOST_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("16.Inner Outline", "Inner Outline", "INNEROUTLINE_ON", "INNEROUTLINE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Hologram", "Hologram", "HOLOGRAM_ON", "HOLOGRAM_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Chromatic Aberration", "ChromAberr", "CHROMABERR_ON", "CHROMABERR_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Glitch", "Glitch", "GLITCH_ON", "GLITCH_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Flicker", "Flicker", "FLICKER_ON", "FLICKER_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Shadow", "Shadow", "SHADOW_ON", "SHADOW_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Hand Drawn (Doodle)", "Hand Drawn", "DOODLE_ON", "DOODLE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Dictionary<string, ShaderPropertyAtributes> grassToggles = new Dictionary<string, ShaderPropertyAtributes>()
        {
            { "Grass is manually animated?", new ShaderPropertyAtributes("MANUALWIND_ON", "_GrassManualToggle") },
        };
        Toggles.Add(new FeatureToggle("0.Grass Wind Movement", "Grass", "WIND_ON", "WIND_OFF", grassToggles));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Wave", "UV Wave", "WAVEUV_ON", "WAVEUV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.Rect Size", "Rect Size", "RECTSIZE_ON", "RECTSIZE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Offset", "UV Offset", "OFFSETUV_ON", "OFFSETUV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Clipping (Cut image)", "UV Clipping", "CLIPPING_ON", "CLIPPING_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("1.UV Scroll (Texture Scroll)", "Texture Scroll", "TEXTURESCROLL_ON", "TEXTURESCROLL_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Zoom (Both IN and OUT)", "UV Zoom", "ZOOMUV_ON", "ZOOMUV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("6.Distortion", "Distortion", "DISTORT_ON", "DISTORT_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Polar Coordinates", "UV Polar", "POLARUV_ON", "POLARUV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Twist", "UV Twist", "TWISTUV_ON", "TWISTUV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Rotate", "UV Rotate", "ROTATEUV_ON", "ROTATEUV_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Fish Eye", "UV Fish Eye", "FISHEYE_ON", "FISHEYE_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Pinch", "UV Pinch", "PINCH_ON", "PINCH_OFF"));

        //------------------------------------------------------------------------------------------------------------

        Toggles.Add(new FeatureToggle("0.UV Shake", "UV Shake", "SHAKEUV_ON", "SHAKEUV_OFF"));

        //------------------------------------------------------------------------------------------------------------
    }
}