using STARTING;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int chip { get; private set; }
    public Sprite chipIcon;

    public static Inventory Instance { get; private set; }

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
