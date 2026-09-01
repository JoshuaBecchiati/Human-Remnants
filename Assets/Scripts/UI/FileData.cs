using System;
using UnityEngine;

[Serializable]
public class FileData
{
    [SerializeField] private int _slotIndex;
    [SerializeField] private bool _isUsed;

    private GameObject _file;

    public int SlotIndex => _slotIndex;
    public GameObject File => _file;
    public bool IsUsed => _isUsed;

    public void SetUsed(bool state) => _isUsed = state;
    public void SetFile(GameObject file) => _file = file;
}
