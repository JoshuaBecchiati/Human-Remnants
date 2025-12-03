using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonHook : MonoBehaviour, IPointerEnterHandler
{
    private IUIButton _buttonHandler;
    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _buttonHandler = GetComponent<IUIButton>();

        _button.onClick.AddListener(OnClickButton);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        VolumeManager.Instance.PlayUIHover();
    }

    public void OnClickButton()
    {
        VolumeManager.Instance.PlayUIClick();

        if (_buttonHandler != null)
        {
            _buttonHandler.OnButtonPressed();
        }
    }
}
