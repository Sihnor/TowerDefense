using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "GraphicSettings", menuName = "Settings/GraphicSettings", order = 1)]
public class GraphicSettings : ScriptableObject
{
    [SerializeField] private int ResolutionWidth;
    [SerializeField] private int ResolutionHeight;
    [SerializeField] private bool FullScreen;
    [SerializeField] private int URPDefaultSettings; // 0 = Performance, 1 = Balanced, 2 = High Fidelity
    
    [SerializeField] private bool HDR;
    [SerializeField] private int AntiAliasing; // 0 = None, 1 = x2, 2 = x4, 3 = x8
    [SerializeField] private int ShadowQuality; // 1 = Performance, 2 = Balanced, 3 = High Fidelity
    
    [Button("Save Graphic Settings")]
    public void SaveSoundSettings()
    {
        string filePath = Application.persistentDataPath + "/GraphicSettings.json";
        string json = JsonUtility.ToJson(this, true);
        System.IO.File.WriteAllText(filePath, json);

        Debug.Log("Graphic Settings saved to: " + filePath);
    }
    
    [Button("Load Graphic Settings")]
    public void LoadSoundSettings()
    {
        string filePath = Application.persistentDataPath + "/GraphicSettings.json";
        if (System.IO.File.Exists(filePath))
        {
            var json = System.IO.File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
            
            Debug.Log("Graphic Settings loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("Graphic Settings file not found in: " + filePath);
        }
    }
    
    [Button("Load Default Graphic Settings")]
    public void LoadDefaultSoundSettings()
    {
        this.ResolutionWidth = 1920;
        this.ResolutionHeight = 1080;
        this.FullScreen = true;
        this.URPDefaultSettings = 1;
        
        this.HDR = true;
        this.AntiAliasing = 2;
        this.ShadowQuality = 2;
        
        Debug.Log("Default Graphic Settings loaded");
    }
    
    public int GetResolutionWidth()
    {
        return this.ResolutionWidth;
    }
    
    public void SetResolutionWidth(int width)
    {
        this.ResolutionWidth = width;
    }
    
    public int GetResolutionHeight()
    {
        return this.ResolutionHeight;
    }
    
    public void SetResolutionHeight(int height)
    {
        this.ResolutionHeight = height;
    }
    
    public bool GetFullScreen()
    {
        return this.FullScreen;
    }
    
    public void SetFullScreen(bool fullScreen)
    {
        this.FullScreen = fullScreen;
    }
    
    public int GetURPDefaultSettings()
    {
        return this.URPDefaultSettings;
    }
    
    public void SetURPDefaultSettings(int settings)
    {
        if (settings != 0 || settings != 1 || settings != 2)
        {
            Debug.LogWarning("URP Default Settings must be 0, 1 or 2");
            return;
        }
        
        this.URPDefaultSettings = settings;
    }
    
    public bool GetHDR()
    {
        return this.HDR;
    }
    
    public void SetHDR(bool hdr)
    {
        this.HDR = hdr;
    }
    
    public int GetAntiAliasing()
    {
        return this.AntiAliasing;
    }
    
    public void SetAntiAliasing(int antiAliasing)
    {
        if (antiAliasing != 0 || antiAliasing != 1 || antiAliasing != 2 || antiAliasing != 3)
        {
            Debug.LogWarning("Anti Aliasing must be 0, 1, 2 or 3");
            return;
        }
        
        this.AntiAliasing = antiAliasing;
    }
    
    public int GetShadowQuality()
    {
        return this.ShadowQuality;
    }
    
    public void SetShadowQuality(int quality)
    {
        if (quality != 1 || quality != 2 || quality != 3)
        {
            Debug.LogWarning("Shadow Quality must be 1, 2 or 3");
            return;
        }
        
        this.ShadowQuality = quality;
    }
}
