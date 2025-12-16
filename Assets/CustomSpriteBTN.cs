using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSpriteBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Normal")]
    [SerializeField] private Image m_deHighlightedImage;
    [Header("Highlithed")]
    [SerializeField] private Image m_highlightedImage;

    private void OnDisable()
    {
        m_deHighlightedImage.gameObject.SetActive(true);
        m_highlightedImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_deHighlightedImage.gameObject.SetActive(false);
        m_highlightedImage.gameObject.SetActive(true);

        VolumeManager.Instance.PlayUIClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_deHighlightedImage.gameObject.SetActive(true);
        m_highlightedImage.gameObject.SetActive(false);
    }
}
