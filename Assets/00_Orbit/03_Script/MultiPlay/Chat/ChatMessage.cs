using TMPro;
using UnityEngine;

namespace STARTING
{
    public class ChatMessage : MonoBehaviour
    {
        public TMP_Text playerNameText;
        public TMP_Text messageText;

        public void SetMessage(string playerName, string message)
        {
            playerNameText.text = playerName;
            messageText.text = message;
        }
    }
}