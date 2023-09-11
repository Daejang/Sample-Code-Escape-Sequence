// Authors: Kalby Jang
// Copyright © 2021 DigiPen - All Rights Reserved

using UnityEngine;
using UnityEngine.UI;

public class PixelateEffectCustomizer : MonoBehaviour
{
    #region Class Members

    [Header("Clients")]
    public Camera[] camerasToPixelate;
    public RawImage displayImage;

    [Header("Settings")]
    public Vector2Int targetScreenResolution = new Vector2Int( 160, 192 );
    
    private Vector2Int savedScreenResolution;

    private RenderTexture pixelatedEffectTexture;

    [Header("SettingsForPixelizationLerp")]
    public bool doingEndTransition = false;
    public Vector2Int deathScreenResolution = new Vector2Int(50, 28);
    public float InterpolationRate = 0.05f;
    private Vector2 currentTargetScreenResolution;

    private RenderTexture prevTexture;
    #endregion
    
    

    #region Unity Methods
    
    void Start()
    {
        MakeRenderTexture();

        //Match the float version of the target screen resolution to the int version
        currentTargetScreenResolution = targetScreenResolution;
    }

    void Update()
    {
        if (CheckForScreenSizeChange() || CheckForTargetResolutionChange()) MakeRenderTexture();

        if (doingEndTransition)
        {
            LerpPixelizationUp();
        }
    }

    #endregion

    #region Class Methods

    void MakeRenderTexture()
    {
        savedScreenResolution.x = Screen.width;
        savedScreenResolution.y = Screen.height;

        targetScreenResolution.x = targetScreenResolution.x < 1 ? 1 : targetScreenResolution.x;
        targetScreenResolution.y = targetScreenResolution.y < 1 ? 1 : targetScreenResolution.y;

        prevTexture = pixelatedEffectTexture;
        
        pixelatedEffectTexture = new RenderTexture( targetScreenResolution.x, targetScreenResolution.y, 24 )
        {
            name = "Pixelated Camera",
            filterMode = FilterMode.Point,
            antiAliasing = 1,
        };


        for (int c = 0; c < camerasToPixelate.Length; c++)
        {
            camerasToPixelate[c].targetTexture = pixelatedEffectTexture;
        }

        displayImage.texture = pixelatedEffectTexture;
        displayImage.color = Color.white;

        if (prevTexture)
        {
            prevTexture.Release();
            prevTexture.DiscardContents();
        }
    }

    bool CheckForScreenSizeChange()
    {
        return Screen.width != savedScreenResolution.x || 
               Screen.height != savedScreenResolution.y;
    }
    bool CheckForTargetResolutionChange()
    {
        return targetScreenResolution.x != savedScreenResolution.x || 
               targetScreenResolution.y != savedScreenResolution.y;
    }

    void LerpPixelizationUp()
    {
        //Track the increasing pixelization value
        currentTargetScreenResolution = Vector2.Lerp(currentTargetScreenResolution, deathScreenResolution, InterpolationRate);
        
        //Apply the new pixelization value
        targetScreenResolution = Vector2Int.RoundToInt(currentTargetScreenResolution);
        
        //Once the target value is reached, stop lerping
        if (targetScreenResolution == deathScreenResolution)
            doingEndTransition = false;
    }
    
    #endregion
}
