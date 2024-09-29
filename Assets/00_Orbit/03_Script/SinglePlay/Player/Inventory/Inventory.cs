using STARTING;
using System.Collections;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int chip { get; private set; }
    public Sprite chipIcon;

    void Start()
    {
        InitializeInventory();
    }

    public void GetChip()
    {
        chip++;
        UIManager.Instance.UpdateStats("chip", chip);
        StartCoroutine(ChipLog(2f));
    }

    public IEnumerator ChipLog(float time)
    {
        yield return new WaitForSeconds(time);
        UIManager.Instance.ShowKillLog("¿ÂÀüÇÑ Ä¨", "È¹µæ", 2f, "blue", chipIcon);
        StartCoroutine(UIManager.Instance.ShowTipKey());
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
}
