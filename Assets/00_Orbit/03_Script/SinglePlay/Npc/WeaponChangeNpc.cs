using UnityEngine;

namespace NOLDA
{
    public class WeaponChangeNpc : MonoBehaviour
    {
        private bool _isPlayerInTrigger = false;

        private void Update()
        {
            if (_isPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.OpenChangeWeapon();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInTrigger = true;
                UIManager.Instance.interactionKeyEnable("º¸±Þ¼Ò", "F");
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
    }
}