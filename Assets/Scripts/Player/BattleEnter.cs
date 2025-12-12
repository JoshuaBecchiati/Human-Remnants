using UnityEngine;

public class BattleEnter : SaveableObject
{
    [SerializeField] private BattleSettings _battleSettings;

    private bool _isDead;

    public bool IsDead => _isDead;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            GameEvents.BattleStart(_battleSettings, gameObject);
        }
    }

    public void OnDeath()
    {
        SaveSystem.Instance.CurrentSave.defeatedEnemies.Add(uniqueID);
    }

    public void SetDeathState(bool state)
    {
        _isDead = state;
    }

    public override void SaveState(SaveData save)
    {
        if (!save.collectedItems.Contains(uniqueID) && !gameObject.activeSelf)
            save.collectedItems.Add(uniqueID);
    }

    public override void LoadState(SaveData save)
    {
        if (save.collectedItems.Contains(uniqueID))
            gameObject.SetActive(false);
        else if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }
}
