using Mirror;

namespace STARTING
{
    public struct LoginRequestMessage : NetworkMessage
    {
        public string username;
        public string password;
    }

    public struct LoginResponseMessage : NetworkMessage
    {
        public bool success;
        public int userId; // 로그인 성공 시 유저 ID를 반환
    }

    public struct GameDataRequestMessage : NetworkMessage
    {
        public int userId; // 유저가 로그인에 성공한 후 게임 데이터를 요청
    }

    public struct GameDataResponseMessage : NetworkMessage
    {
        public GameData gameData; // DB에서 불러온 게임 데이터
    }

    public struct SaveGameMessage : NetworkMessage
    {
        public GameData gameData; // 저장할 게임 데이터
    }

    public struct RegisterRequestMessage : NetworkMessage
    {
        public string username;
        public string password;
    }

    public struct RegisterResponseMessage : NetworkMessage
    {
        public bool success;
        public string message; // 회원가입 실패 시 에러 메시지를 전달
    }
}