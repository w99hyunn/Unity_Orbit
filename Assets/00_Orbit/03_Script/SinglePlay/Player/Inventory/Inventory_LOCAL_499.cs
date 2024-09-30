using Cysharp.Threading.Tasks;
using STARTING;
using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int chip { get; private set; }
    public Sprite chipIcon;

    void Start()
    {
        InitializeInventory();
    }

    public async UniTask GainChip()
    {
        chip++;
        GameManager.Instance.SaveGamePartial("chip", chip);
        UIManager.Instance.UpdateStats("chip", chip);
        await ChipLog(2f);
    }

    public async UniTask ChipLog(float time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        UIManager.Instance.ShowKillLog("온전한 칩", "획득", 2f, "blue", chipIcon);
        await UIManager.Instance.ShowTipKey(); // ShowTipKey()도 비동기 메서드라고 가정
    }

    public void InitializeInventory()
    {
        chip = 0;
    }

    public void SetInventory(int chip)
    {
        this.chip = chip;
        UIManager.Instance.UpdateStats("chip", chip);
    }

    public int GetChip() { return chip; }
}
