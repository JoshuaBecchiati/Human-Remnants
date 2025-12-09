using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Battle/Attack")]

public class Attack : ScriptableObject
{
    [SerializeField] private string m_name = "Attack name";
    [SerializeField] private float m_damage = 10f;
    [SerializeField, Range(0, 100)] private int  m_possibility = 50;
    [SerializeField] private GameObject m_animation;

    public string Name => m_name;
    public float Damage => m_damage;
    public int Possibility => m_possibility;
    public GameObject Animation => m_animation;
}
