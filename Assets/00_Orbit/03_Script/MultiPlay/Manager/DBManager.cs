using MySql.Data.MySqlClient;

using System;
using System.Collections;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace NOLDA
{
    public class DBManager : MonoBehaviour
    {
        private const string DEFAULT_SERVER_IP = "localhost";
        private const string DEFAULT_PORT = "3306";
        private const string DEFAULT_HOST = "root";
        private const string DEFAULT_PASSWORD = "root";

        private string DBSERVER_IP;
        private string DBPORT;
        private string DBHOST;
        private string DBPW;

        public static DBManager Instance;

        private MySqlConnection connection;

        [Header("Database Configuration")]
        [SerializeField] private DBConfigSO dbConfig;

        [Header("클라이언트가 각자 가지고 있는 자신의 정보")]
        public GameData clientGameData;
        public string userName;
        public int userId;

        void Awake()
        {
            if (!InitializeSingleton()) return;
            LoadDBConfig();
        }

        private void LoadDBConfig()
        {
            if (dbConfig != null)
            {
                DBSERVER_IP = string.IsNullOrEmpty(dbConfig.serverIP) ? DEFAULT_SERVER_IP : dbConfig.serverIP;
                DBPORT = string.IsNullOrEmpty(dbConfig.port) ? DEFAULT_PORT : dbConfig.port;
                DBHOST = string.IsNullOrEmpty(dbConfig.host) ? DEFAULT_HOST : dbConfig.host;
                DBPW = string.IsNullOrEmpty(dbConfig.password) ? DEFAULT_PASSWORD : dbConfig.password;

                Debug.Log("DB Config loaded from ScriptableObject.");
                return;
            }

            // ScriptableObject가 없을 경우 기본값 사용
            DBSERVER_IP = DEFAULT_SERVER_IP;
            DBPORT = DEFAULT_PORT;
            DBHOST = DEFAULT_HOST;
            DBPW = DEFAULT_PASSWORD;

            Debug.LogWarning("DBConfig ScriptableObject is not assigned. Using default DB config values.");
        }

        bool InitializeSingleton()
        {
            if (Instance != null && Instance == this)
                return true;


            if (Instance != null)
            {
                Destroy(gameObject);
                return false;
            }
            Instance = this;

            if (Application.isPlaying)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            return true;
        }

        public IEnumerator ConnectDB()
        {
            yield return StartCoroutine(ConnectToDatabase(DBSERVER_IP, "orbit", DBHOST, DBPW, DBPORT));
        }

        public IEnumerator ConnectToDatabase(string server, string database, string uid, string password, string port)
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};PORT={port};Connection Timeout=20;Allow Zero Datetime=True;Convert Zero Datetime=True;SslMode=Preferred;AllowPublicKeyRetrieval=true;";
            connection = new MySqlConnection(connectionString);

            Task connectTask = Task.Run(async () =>
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch
                { }
            });

            while (!connectTask.IsCompleted)
            {
                yield return null;
            }
        }

        public bool IsConnected()
        {
            return connection != null && connection.State == System.Data.ConnectionState.Open;
        }


        public void Register(string username, string password)
        {
            // 비밀번호 해시 및 솔트 생성
            var (salt, hash) = HashPassword(password);
            string query = "INSERT INTO users (username, password_hash, password_salt) VALUES (@username, @password_hash, @password_salt)";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password_hash", Convert.ToBase64String(hash)); // 해시를 Base64로 변환
                cmd.Parameters.AddWithValue("@password_salt", Convert.ToBase64String(salt)); // 솔트를 Base64로 변환
                cmd.ExecuteNonQuery();
            }
        }


        public bool Login(string username, string password, out int userId)
        {
            userId = 0;
            string query = "SELECT user_id, password_hash, password_salt FROM users WHERE username = @username"; // Salt 추가
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPasswordHash = reader.GetString("password_hash");
                        byte[] storedPasswordSalt = Convert.FromBase64String(reader.GetString("password_salt")); // Salt를 Base64로 변환하여 가져옴
                        userId = reader.GetInt32("user_id");

                        // 비밀번호 비교 (해싱 알고리즘에 맞게 구현)
                        if (VerifyPasswordHash(password, storedPasswordSalt, Convert.FromBase64String(storedPasswordHash))) // Hash도 Base64로 변환하여 비교
                        {
                            return true;
                        }
                    }
                }
            }

            Debug.LogError("Login failed.");
            return false;
        }

        public GameData GetGameDataFromDB(int userId)
        {
            GameData gameData = new GameData();

            string query = "SELECT * FROM game_data WHERE user_id = @userId";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        gameData.gameTime = reader.GetFloat("game_time");
                        gameData.maxHealth = reader.GetInt32("max_health");
                        gameData.maxMana = reader.GetInt32("max_mana");
                        gameData.maxExperience = reader.GetInt32("max_experience");

                        gameData.currentHealth = reader.GetInt32("current_health");
                        gameData.currentMana = reader.GetInt32("current_mana");
                        gameData.currentExperience = reader.GetInt32("current_experience");

                        gameData.level = reader.GetInt32("level");
                        gameData.playerPosition = new Vector3(
                            reader.GetFloat("player_position_x"),
                            reader.GetFloat("player_position_y"),
                            reader.GetFloat("player_position_z")
                        );
                        gameData.chip = reader.GetInt32("chip");

                        return gameData;
                    }
                    else
                    {
                        //클라이언트에게 넘어간 데이터의 level 값이 -1일 경우 DB에 데이터가 없다는 의미
                        gameData.level = -1;
                        return gameData;
                    }
                }
            }

        }

        public void UpdateGameDataInDB(int userId, GameData gameData)
        {
            string updateQuery = @"
                                UPDATE game_data 
                                SET 
                                    game_time = @gameTime,
                                    max_health = @maxHealth,
                                    max_mana = @maxMana,
                                    max_experience = @maxExperience,
                                    current_health = @currentHealth,
                                    current_mana = @currentMana,
                                    current_experience = @currentExperience,
                                    level = @level,
                                    player_position_x = @posX,
                                    player_position_y = @posY,
                                    player_position_z = @posZ,
                                    chip = @chip
                                WHERE user_id = @userId";

            using (MySqlCommand cmd = new MySqlCommand(updateQuery, connection))
            {
                cmd.Parameters.AddWithValue("@gameTime", gameData.gameTime);
                cmd.Parameters.AddWithValue("@maxHealth", gameData.maxHealth);
                cmd.Parameters.AddWithValue("@maxMana", gameData.maxMana);
                cmd.Parameters.AddWithValue("@maxExperience", gameData.maxExperience);
                cmd.Parameters.AddWithValue("@currentHealth", gameData.currentHealth);
                cmd.Parameters.AddWithValue("@currentMana", gameData.currentMana);
                cmd.Parameters.AddWithValue("@currentExperience", gameData.currentExperience);
                cmd.Parameters.AddWithValue("@level", gameData.level);
                cmd.Parameters.AddWithValue("@posX", gameData.playerPosition.x);
                cmd.Parameters.AddWithValue("@posY", gameData.playerPosition.y);
                cmd.Parameters.AddWithValue("@posZ", gameData.playerPosition.z);
                cmd.Parameters.AddWithValue("@chip", gameData.chip);
                cmd.Parameters.AddWithValue("@userId", userId);

                // 업데이트 실행 및 업데이트된 행의 수 확인
                int rowsAffected = cmd.ExecuteNonQuery();

                // DB에 없는 유저일 경우 새롭게 삽입
                if (rowsAffected == 0)
                {
                    string insertQuery = @"
                                        INSERT INTO game_data (
                                            user_id, game_time, max_health, max_mana, max_experience,
                                            current_health, current_mana, current_experience, level,
                                            player_position_x, player_position_y, player_position_z, chip
                                        ) VALUES (
                                            @userId, @gameTime, @maxHealth, @maxMana, @maxExperience,
                                            @currentHealth, @currentMana, @currentExperience, @level,
                                            @posX, @posY, @posZ, @chip
                                        )";

                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection))
                    {
                        // 같은 파라미터를 사용하여 INSERT 쿼리 실행
                        insertCmd.Parameters.AddWithValue("@gameTime", gameData.gameTime);
                        insertCmd.Parameters.AddWithValue("@maxHealth", gameData.maxHealth);
                        insertCmd.Parameters.AddWithValue("@maxMana", gameData.maxMana);
                        insertCmd.Parameters.AddWithValue("@maxExperience", gameData.maxExperience);
                        insertCmd.Parameters.AddWithValue("@currentHealth", gameData.currentHealth);
                        insertCmd.Parameters.AddWithValue("@currentMana", gameData.currentMana);
                        insertCmd.Parameters.AddWithValue("@currentExperience", gameData.currentExperience);
                        insertCmd.Parameters.AddWithValue("@level", gameData.level);
                        insertCmd.Parameters.AddWithValue("@posX", gameData.playerPosition.x);
                        insertCmd.Parameters.AddWithValue("@posY", gameData.playerPosition.y);
                        insertCmd.Parameters.AddWithValue("@posZ", gameData.playerPosition.z);
                        insertCmd.Parameters.AddWithValue("@chip", gameData.chip);
                        insertCmd.Parameters.AddWithValue("@userId", userId);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // 비밀번호 해시 생성
        public (byte[] salt, byte[] hash) HashPassword(string password)
        {
            // 16바이트 랜덤 솔트 생성
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // PBKDF2 해시 생성
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20); // 20바이트 해시
                return (salt, hash);
            }
        }

        // 비밀번호 해시 검증
        public bool VerifyPasswordHash(string password, byte[] salt, byte[] hash)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                byte[] testHash = pbkdf2.GetBytes(20);
                return StructuralComparisons.StructuralEqualityComparer.Equals(testHash, hash);
            }
        }

        public void CloseDBServer()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

        private void OnApplicationQuit()
        {
            CloseDBServer();
        }
    }
}