using Demo.Scripts.Runtime.Character;
using Mirror;
using System.Collections;
using UnityEngine;

namespace NOLDA
{
    /// <summary>
    /// 맵의 존 트리거에 닿았을 때 출력 및 GameManager에 전달
    /// </summary>
    public class ZoneEntrance_Multi : ZoneEntrance
    {
        private GameObject localPlayer;

        private void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }

        private IEnumerator FindLocalPlayer()
        {
            while (GameManager_Multi.Instance.player == null)
            {
                yield return null;
            }

            localPlayer = GameManager_Multi.Instance.player;
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == localPlayer)
            {
                //멀티플레이의 경우 점령 시스템은 아직 적용되지 않았음.
                //싱글플레이와 동일하게 적용할지, 다른 시스템을 적용할지 고민중
                //bool isLiberated = GameManager.Instance.SetCurrentZone(zoneName);
                UIManager.Instance.UpdateZoneInfo(zoneName, false, true);
            }
        }
    }
}