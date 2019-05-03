using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstBox {

    public static readonly string poolIngame = "InGame";
    public static readonly string poolSpriteEffect = "SpriteEffect";

    
    public static readonly string KeySavedTileHistory = "SavedTileHistory"; // 타일 히스토리 저장 키 
    public static readonly string KeySavedPlayTheme = "SavedPlayTheme"; // 플레이중이던 theme
    public static readonly string KeySavedSurprisePack = "SavedPackDay"; // 서프라이즈 팩 오픈일자

    public static readonly string ParticleConfettiRainbow = "ConfettiBlastRainbow";
    public static readonly string ParticleConfettiGreenYellow = "ConfettiBlastGreenYellow";
    public static readonly string ParticleConfettiOrangeGreen = "ConfettiBlastOrangeGreen";

    public static readonly string SpriteEffectTwinkleStar = "TwinkleStar";
    public static readonly string SpriteEffectShootingStar = "ShootingStar";



    public static readonly Color32 colorOrigin = new Color32(255, 255, 255, 255);
    public static readonly Color32 colorTransparent = new Color32(255, 255, 255, 0);
}
