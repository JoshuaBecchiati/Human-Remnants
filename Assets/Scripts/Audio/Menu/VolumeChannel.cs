using UnityEngine;
using UnityEngine.Audio;

public class VolumeChannel : IVolumeChannel
{
    private readonly string _prefsKey;
    private readonly AudioMixer _mixer;
    private readonly string _mixerParameter;

    public VolumeType Type { get; private set; }
    public float Volume { get; private set; }

    public VolumeChannel(VolumeType type, AudioMixer mixer, string mixerParam)
    {
        Type = type;
        _prefsKey = $"Volume_{type}";
        _mixer = mixer;
        _mixerParameter = mixerParam;
    }

    public void Load()
    {
        Volume = PlayerPrefs.GetFloat(_prefsKey, 0.5f);
        ApplyVolume();
    }

    public void Save()
    {
        PlayerPrefs.SetFloat(_prefsKey, Volume);
    }

    public void SetVolume(float value)
    {
        Volume = Mathf.Clamp01(value);
        ApplyVolume();
        Save();
    }

    private void ApplyVolume()
    {
        // Conversione lineare → dB
        float dB = Mathf.Log10(Mathf.Max(Volume, 0.0001f)) * 20f;
        _mixer.SetFloat(_mixerParameter, dB);
    }
}
