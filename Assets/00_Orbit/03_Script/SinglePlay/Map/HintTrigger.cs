using UnityEngine;

namespace NOLDA
{
    public class HintTrigger : MonoBehaviour
    {
        public string infoText;

        private string currentZoneName;
        private bool isLiberated;
        private Collider triggerCollider;

        private void Start()
        {
            triggerCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                currentZoneName = GameManager.Instance.currentZoneName;
                isLiberated = GameManager.Instance.IsZoneLiberated(currentZoneName);
            }
            if (isLiberated == false && other.CompareTag("Player"))
            {
                UIManager.Instance.ScriptText_Enable(infoText);
            }
        }

        /// <summary>
        /// 힌트 트리거 범위에 Enemy가 있을 경우 던전 입장 불가
        /// </summary>
        /// <returns></returns>
        public bool IsEnemyInTrigger()
        {
            if (triggerCollider == null) return false;

            Collider[] colliders = Physics.OverlapBox(triggerCollider.bounds.center, triggerCollider.bounds.extents, transform.rotation);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}