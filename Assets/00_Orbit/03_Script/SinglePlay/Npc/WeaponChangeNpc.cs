using STARTING;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponChangeNpc : MonoBehaviour
{
    public List<GameObject> itemList;
    public DissolveChilds dissolveChilds;

    //public delegate void DestroyedEventHandler();
    //public event DestroyedEventHandler OnDestroyed;

    [Header("루트 사운드")]
    public AudioClip rootSound;
    public AudioSource audioSource;

    private bool _isPlayerInTrigger = false;

    private void Update()
    {
        if (_isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            UIManager.Instance.OpenChangeWeapon();
            //StartCoroutine(dissolveChilds.AnimateDissolve());
            //PlaySound(rootSound);
            //StartCoroutine(DestroyAfterDelay(1f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = true;
            UIManager.Instance.interactionKeyEnable("무기 상점", "F");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInTrigger = false;
            UIManager.Instance.interactionKeyDisable();
        }
    }

    //private IEnumerator DestroyAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    //루트박스에서 F를 누르면 랜덤 아이템 생성
    //    int randomIndex = Random.Range(0, itemList.Count);
    //    GameObject selectedItem = itemList[randomIndex];

    //    // 현재 위치에 y값 +1해서 스폰
    //    Vector3 currentPosition = this.gameObject.transform.position;
    //    Vector3 newPosition = new Vector3(currentPosition.x, currentPosition.y + 1, currentPosition.z);
    //    Instantiate(selectedItem, newPosition, this.gameObject.transform.rotation);

    //    OnDestroyed?.Invoke();
    //    Destroy(gameObject);
    //}

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}