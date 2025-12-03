using UnityEngine;

[CreateAssetMenu(fileName = "New Novel Scene", menuName = "Visual Novel Engine/Novel Scene")]
public class NovelScene : ScriptableObject
{
    [SerializeField] private string _sceneName;
    [SerializeField] private NovelDialogue _startingDialogue;
    

    public string SceneName => _sceneName;
    public NovelDialogue StartingDialogue => _startingDialogue;
}
