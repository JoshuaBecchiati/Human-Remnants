using UnityEngine;

public interface IPanel
{
    private PanelType Panel { get; set; }

    void OnPanelChange(GameObject newPanel, GameObject previousPanel);
}
