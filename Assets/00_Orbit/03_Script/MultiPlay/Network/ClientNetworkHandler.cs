using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace STARTING
{
    public class ClientNetworkHandler : MonoBehaviour
    {
        public UnityEvent loginSuccess;
        public UnityEvent loginFail;
        public UnityEvent registerSuccess;
        public UnityEvent registerFail;

        public void SendLoginRequest(string username, string password)
        {
            NetworkClient.RegisterHandler<LoginResponseMessage>(OnLoginResponse);

            LoginRequestMessage loginRequest = new LoginRequestMessage
            {
                username = username,
                password = password
            };

            NetworkClient.Send(loginRequest);
        }

        private void OnLoginResponse(LoginResponseMessage msg)
        {
            if (msg.success)
            {
                Debug.Log("Login successful! User ID: " + msg.userId);
                PlayerPrefs.SetInt("UserID", msg.userId);
                loginSuccess?.Invoke();
            }
            else
            {
                loginFail?.Invoke();
                Debug.LogError("Login failed.");
            }
        }



        //






        //

        public void SendRegisterRequest(string username, string password)
        {
            NetworkClient.RegisterHandler<RegisterResponseMessage>(OnRegisterResponse);

            RegisterRequestMessage registerRequest = new RegisterRequestMessage
            {
                username = username,
                password = password
            };

            NetworkClient.Send(registerRequest);
        }

        private void OnRegisterResponse(RegisterResponseMessage msg)
        {
            if (msg.success)
            {
                registerSuccess?.Invoke();
                Debug.Log(msg.message); // 회원가입 성공 메시지
            }
            else
            {
                registerFail?.Invoke();
                Debug.LogError(msg.message); // 실패 시 에러 메시지
            }
        }

        //


    }
}