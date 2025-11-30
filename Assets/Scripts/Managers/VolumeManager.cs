using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer globalMixer;

    [Header("UI Sounds")]
    [SerializeField] private AudioSource m_uiSource;
    [SerializeField] private AudioClip m_clickClip;
    [SerializeField] private AudioClip m_hoverClip;

    private Dictionary<VolumeType, IVolumeChannel> _channels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeChannels();
    }

    private void InitializeChannels()
    {
        _channels = new Dictionary<VolumeType, IVolumeChannel>()
        {
            { VolumeType.Master,    new VolumeChannel(VolumeType.Master, globalMixer, "MasterVol") },
            { VolumeType.Music,     new VolumeChannel(VolumeType.Music, globalMixer, "MusicVol") },
            { VolumeType.UI,        new VolumeChannel(VolumeType.UI, globalMixer, "UIVol") }
        };

        foreach (var c in _channels.Values)
            c.Load();
    }


    public float GetVolume(VolumeType type)
    {
        return _channels[type].Volume;
    }

    public void SetVolume(VolumeType type, float value)
    {
        _channels[type].SetVolume(value);
    }

    public void ChangeVolume(VolumeType type, float delta)
    {
        var c = _channels[type];
        c.SetVolume(c.Volume + delta);
    }

    // --- UI Sound Methods ---
    public void PlayUIClick()
    {
        if (m_uiSource && m_clickClip)
        {
            m_uiSource.PlayOneShot(m_clickClip, GetVolume(VolumeType.UI));
        }
    }

    public void PlayUIHover()
    {
        if (m_uiSource && m_hoverClip)
        {
            m_uiSource.PlayOneShot(m_hoverClip, GetVolume(VolumeType.UI));
        }
    }
}
