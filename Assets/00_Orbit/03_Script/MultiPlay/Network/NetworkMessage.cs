using Mirror;

namespace NOLDA
{
    // 로그인
    public struct LoginRequestMessage : NetworkMessage
    {
        public string username;
        public string password;
    }

    public struct LoginResponseMessage : NetworkMessage
    {
        public bool success;
        public string userName;
        public int userId;
    }

    // 회원가입
    public struct RegisterRequestMessage : NetworkMessage
    {
        public string username;
        public string password;
    }

    public struct RegisterResponseMessage : NetworkMessage
    {
        public bool success;
    }

    // 게임 데이터 로드
    public struct GameDataRequestMessage : NetworkMessage
    {
        public int userId;
    }

    public struct GameDataResponseMessage : NetworkMessage
    {
        public GameData gameData;
    }

    // 게임 데이터 저장
    public struct GameDataUpdateRequestMessage : NetworkMessage
    {
        public GameData gameData;
        public int userId;
    }
}