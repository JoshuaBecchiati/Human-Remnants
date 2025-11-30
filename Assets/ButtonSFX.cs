using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickButton);
    }

    public void OnClickButton()
    {
        VolumeManager.Instance.PlayUIClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        VolumeManager.Instance.PlayUIHover();
    }
}
