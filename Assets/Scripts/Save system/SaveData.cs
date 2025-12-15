using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public PlayerData player;
    public List<MemberData> party = new();
    public List<ItemData> inventory = new();
    public List<string> collectedItems = new();
    public List<string> defeatedEnemies = new();
    public List<string> completedEvents = new();

    public string currentScene;
    public float totalPlayTime;
    public string lastSaveDate;
}