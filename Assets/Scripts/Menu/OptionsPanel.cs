using UnityEngine;

public class OptionsPanel : UIPanel
{
    [SerializeField] private UIPanel m_audioPanel;
    //[SerializeField] private UIPanel videoPanel;

    public void OpenAudio() 
    {
        MenuManager.Instance.OpenPanel(m_audioPanel);
    }

    //public void OpenVideo() { MenuManager.Instance.OpenPanel(videoPanel); }
}
