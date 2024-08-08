using UnityEngine;

namespace Michsky.UI.Shift
{
    public class ExitToSystem : MonoBehaviour
    {
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
        }
    }
}