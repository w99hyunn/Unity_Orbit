using Mirror;
using System.Collections;
using UnityEngine;

namespace NOLDA
{
    public class PlayerHeadRenderer : NetworkBehaviour
    {
        private SkinnedMeshRenderer headRenderer;

        void Start()
        {
            StartCoroutine(FindLocalPlayer());
        }
        private IEnumerator FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                yield return null;
            }

            headRenderer = GetComponent<SkinnedMeshRenderer>();

            if (isLocalPlayer)
            {
                headRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
            else
            {
                headRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }
    }
}