using UnityEngine;

[CreateAssetMenu(fileName = "New Scene", menuName = "Dialogue system/Novel Scene")]
public class DialogueScene : ScriptableObject
{
    [SerializeField] private string _sceneName;
    [SerializeField] private Dialogue _startingDialogue;

    public string SceneName => _sceneName;
    public Dialogue StartingDialogue => _startingDialogue;
}
