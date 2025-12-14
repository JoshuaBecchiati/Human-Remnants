using UnityEngine;

public class PlayerSave : SaveableObject
{
    [SerializeField] private Player m_player;
    [SerializeField] private Transform m_playerTransform;

    private void OnValidate()
    {
        if (!m_player) m_player = transform.parent.GetComponent<Player>();
        if (!m_playerTransform) m_playerTransform = GetComponent<Transform>();
    }

    public override void SaveState(SaveData save)
    {
        if (save.player == null)
            save.player = new PlayerData();

        save.player.characterID = uniqueID;
        save.player.x = m_playerTransform.localPosition.x;
        save.player.y = m_playerTransform.localPosition.y;
        save.player.z = m_playerTransform.localPosition.z;
        save.player.rotation = m_playerTransform.localEulerAngles.y;
        save.player.hp = m_player.Health;
    }

    public override void LoadState(SaveData save)
    {
        if (save.player == null) return;

        Debug.Log("Load player state");

        CharacterController cc = m_player.transform.Find("Model").GetComponent<CharacterController>();
        cc.enabled = false; // disattivo temporaneamente
        m_playerTransform.position = new Vector3(save.player.x, save.player.y, save.player.z);
        m_playerTransform.rotation = Quaternion.Euler(0f, save.player.rotation, 0f);
        cc.enabled = true;

        // Setto HP
        m_player.SetHealth(save.player.hp);
    }
}
