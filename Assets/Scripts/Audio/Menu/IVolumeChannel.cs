public interface IVolumeChannel
{
    VolumeType Type { get; }
    float Volume { get; }
    void SetVolume(float value);
    void Load();
    void Save();
}
