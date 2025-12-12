using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public PlayerData player; // Fatto
    public GameObject[] party = new GameObject[3];
    public List<ItemData> inventory = new(); // Fatto
    public List<string> collectedItems = new(); // Fatto
    public List<string> defeatedEnemies = new(); // Fatto
    public List<string> completedEvents = new();

    public string currentScene;
    public float totalPlayTime;
    public string lastSaveDate;
}