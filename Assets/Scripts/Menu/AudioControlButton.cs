using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioControlButton : Selectable
{
    [Header("Riferimenti")]
    [SerializeField] private Image m_backgroundImage;
    [SerializeField] private Sprite m_selectedSprite;

    [Header("Bottoni")]
    [SerializeField] private Button m_increaseButton;
    [SerializeField] private Button m_decreaseButton;
    [SerializeField] private Image m_increaseButtonImage;
    [SerializeField] private Image m_decreaseButtonImage;
    [SerializeField] private Sprite m_normalButtonSprite;
    [SerializeField] private Sprite m_selectedButtonSprite;

    [Header("Text")]
    [SerializeField] private Image m_textAudio;
    [SerializeField] private Sprite m_selectedTextSprite;
    [SerializeField] private Sprite m_normalTextSprite;

    [Header("Barrette volume")]
    [SerializeField] private Image[] m_volumeBars;
    [SerializeField] private Sprite[] m_barNormal;
    [SerializeField] private Sprite[] m_barHighlighted;

    [Header("Volume settings")]
    [SerializeField] private VolumeType volumeType;
    [SerializeField] private float volumeStep = 0.1f;

    private bool _isActive = false;
    private bool _isHover = false;
    private int _volume = 5;
    private int _selected = 5;

    protected override void Start()
    {
        base.Start();

        m_increaseButton.transition = Transition.None;
        m_decreaseButton.transition = Transition.None;

        m_increaseButton.onClick.AddListener(() => ChangeVolume(+1));
        m_decreaseButton.onClick.AddListener(() => ChangeVolume(-1));

        InitializeFromSavedVolume();

        UpdateVisuals();
    }

    private void InitializeFromSavedVolume()
    {
        float saved = VolumeManager.Instance.GetVolume(volumeType);  // 0..1
        _volume = Mathf.RoundToInt(saved * m_volumeBars.Length);     // converto 0-1 in barre
        _selected = _volume;
    }

    //public override void OnSelect(BaseEventData eventData)
    //{
    //    base.OnSelect(eventData);
    //    VolumeManager.Instance.PlayUIClick();
    //    _isActive = true;
    //    UpdateVisuals();
    //}

    //public override void OnDeselect(BaseEventData eventData)
    //{
    //    base.OnDeselect(eventData);
    //    _isActive = false;
    //    UpdateVisuals();
    //}

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        VolumeManager.Instance.PlayUIHover();
        _isHover = true;
        UpdateVisuals();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _isHover = false;
        UpdateVisuals();
    }

    private void ChangeVolume(int dir)
    {
        _volume = Mathf.Clamp(_volume + dir, 0, m_volumeBars.Length);
        _selected = _volume;
        UpdateVisuals();
    }

    public void IncreaseVolume()
    {
        VolumeManager.Instance.ChangeVolume(volumeType, +volumeStep);
    }

    public void DecreaseVolume()
    {
        VolumeManager.Instance.ChangeVolume(volumeType, -volumeStep);
    }

    private void UpdateVisuals()
    {
        // Sprite del pulsante principale
        if (_isActive || _isHover)
        {
            SetAlpha(1f, m_backgroundImage);
            m_backgroundImage.sprite = m_selectedSprite;
        }
        else
        {
            SetAlpha(0f, m_backgroundImage);
        }

        if (m_textAudio)
            m_textAudio.sprite = _isActive || _isHover
                ? m_selectedTextSprite
                : m_normalTextSprite;

        // Bottoni (+) e (–) seguono lo stato della sezione
        if (m_increaseButtonImage)
            m_increaseButtonImage.sprite = _isActive || _isHover
                ? m_selectedButtonSprite
                : m_normalButtonSprite;

        if (m_decreaseButtonImage)
            m_decreaseButtonImage.sprite = _isActive || _isHover
                ? m_selectedButtonSprite
                : m_normalButtonSprite;

        // Barre volume
        for (int i = 0; i < m_volumeBars.Length; i++)
        {
            if (i < _volume)
            {
                SetAlpha(1f, m_volumeBars[i]);
                m_volumeBars[i].sprite = _isActive || _isHover
                    ? m_barHighlighted[i]
                    : m_barNormal[i];
            }
            else
            {
                SetAlpha(0f, m_volumeBars[i]);
            }
        }
    }

    public void SetAlpha(float alpha, Image image)
    {
        // Prende l’immagine del bottone
        Image img = image;

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
