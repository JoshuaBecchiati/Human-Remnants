using UnityEngine;

public class NewGameBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    [SerializeField] private Characters m_character;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        Time.timeScale = 1f;
        MenuManager.Instance.NewGame(m_character);
    }
}
