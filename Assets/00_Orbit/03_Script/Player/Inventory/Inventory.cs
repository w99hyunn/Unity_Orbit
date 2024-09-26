using STARTING;
using System.Collections;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public int chip { get; private set; }
    public Sprite chipIcon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeInventory();
    }

    public void GetChip()
    {
        chip++;
        UIManager.Instance.UpdateStats("chip", chip);
        UIManager.Instance.ShowKillLog("¿ÂÀüÇÑ Ä¨", "È¹µæ", 2f, "blue", chipIcon);
        StartCoroutine(ShowTipKey());
    }

    IEnumerator ShowTipKey()
    {
        UIManager.Instance.tipKeyEnable("»ó¼¼Á¤º¸", "TAB");

        yield return new WaitForSeconds(3f);

        UIManager.Instance.tipKeyDisable();
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
