using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public PlayerData player; // Fatto
    public List<PartyMemberData> party = new();
    public List<ItemData> inventory = new(); // Fatto
    public List<string> collectedItems = new(); // Fatto
    public List<string> defeatedEnemies = new(); // Fatto
    public List<string> completedEvents = new();

    public float totalPlayTime;
    public string creationDate;
}