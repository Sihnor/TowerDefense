using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundSettings", menuName = "Settings/SoundSettings", order = 1)]
public class SoundSettings : ScriptableObject
{
    [SerializeField] private float MasterVolume;
    [SerializeField] private float MusicVolume;
    [SerializeField] private float VoiceVolume;
    [SerializeField] private float SFXVolume;
    [SerializeField] private float AmbientVolume;
    
    [Button("Save Sound Settings")]
    public void SaveSoundSettings()
    {
        string filePath = Application.persistentDataPath + "/SoundSettings.json";
        string json = JsonUtility.ToJson(this, true);
        System.IO.File.WriteAllText(filePath, json);
        
        Debug.Log("Sound Settings saved to: " + filePath);
    }
    
    [Button("Load Sound Settings")]
    public void LoadSoundSettings()
    {
        string filePath = Application.persistentDataPath + "/SoundSettings.json";
        if (System.IO.File.Exists(filePath))
        {
            var json = System.IO.File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
            
            Debug.Log("Sound Settings loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("Sound Settings file not found in: " + filePath);
        }
    }
    
    [Button("Load Default Sound Settings")]
    public void LoadDefaultSoundSettings()
    {
        this.MasterVolume = 1;
        this.MusicVolume = 1;
        this.VoiceVolume = 1;
        this.SFXVolume = 1;
        this.AmbientVolume = 1;
        
        Debug.Log("Default Sound Settings loaded");
    }
    
    public float GetMasterVolume()
    {
        return this.MasterVolume;
    }
    
    public void SetMasterVolume(float volume)
    {
        this.MasterVolume = volume;
    }
    
    public float GetMusicVolume()
    {
        return this.MusicVolume;
    }
    
    public void SetMusicVolume(float volume)
    {
        this.MusicVolume = volume;
    }
    
    public float GetVoiceVolume()
    {
        return this.VoiceVolume;
    }
    
    public void SetVoiceVolume(float volume)
    {
        this.VoiceVolume = volume;
    }
    
    public float GetSFXVolume()
    {
        return this.SFXVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        this.SFXVolume = volume;
    }
    
    public float GetAmbientVolume()
    {
        return this.AmbientVolume;
    }
    
    public void SetAmbientVolume(float volume)
    {
        this.AmbientVolume = volume;
    }
}
