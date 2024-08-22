using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBox : MonoBehaviour
{
    private bool isPlayerInTrigger = false;
    public List<GameObject> itemList;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            UIManager.Instance.interactionKeyEnable("상자 열기", "F");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            UIManager.Instance.interactionKeyDisable();
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            //루트박스에서 F를 누르면 랜덤 아이템 생성
            int randomIndex = Random.Range(0, itemList.Count);
            GameObject selectedItem = itemList[randomIndex];

            // 현재 위치에 y값 +1해서 스폰
            Vector3 currentPosition = this.gameObject.transform.position;
            Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y + 1, currentPosition.z);
            Instantiate(selectedItem, newPosition, this.gameObject.transform.rotation);

            UIManager.Instance.interactionKeyDisable();
            Destroy(this.gameObject);
        }
    }
}
