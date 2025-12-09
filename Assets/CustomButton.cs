using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Normal")]
    [SerializeField] private TextMeshProUGUI m_normalText;
    [Header("Highlithed")]
    [SerializeField] private TextMeshProUGUI m_highlightedText;
    [SerializeField] private Image m_highlightedImage;

    private void OnValidate()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_normalText.gameObject.SetActive(false);

        m_highlightedText.gameObject.SetActive(true);
        m_highlightedImage.gameObject.SetActive(true);

        VolumeManager.Instance.PlayUIClick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_normalText.gameObject.SetActive(true);

        m_highlightedText.gameObject.SetActive(false);
        m_highlightedImage.gameObject.SetActive(false);
    }
}
