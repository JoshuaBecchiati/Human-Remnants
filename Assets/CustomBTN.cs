using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Normal")]
    [SerializeField] private List<TextMeshProUGUI> m_normalTexts;
    [Header("Highlithed")]
    [SerializeField] private List<TextMeshProUGUI> m_highlightedTexts;
    [SerializeField] private Image m_highlightedImage;

    private void OnDisable()
    {
        foreach (var text in m_normalTexts)
            text.gameObject.SetActive(true);

        foreach (var text in m_highlightedTexts)
            text.gameObject.SetActive(false);

        m_highlightedImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (var text in m_normalTexts)
            text.gameObject.SetActive(false);

        foreach (var text in m_highlightedTexts)
            text.gameObject.SetActive(true);

        m_highlightedImage.gameObject.SetActive(true);

        VolumeManager.Instance.PlayUIClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (var text in m_normalTexts)
            text.gameObject.SetActive(true);

        foreach (var text in m_highlightedTexts)
            text.gameObject.SetActive(false);
        m_highlightedImage.gameObject.SetActive(false);
    }
}
