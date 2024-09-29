using MySql.Data.MySqlClient;

using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Device;

namespace STARTING
{
    public class DBManager : MonoBehaviour
    {
        public static DBManager Instance;

        private MySqlConnection connection;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ConnectDB()
        {
            ConnectToDatabase("localhost", "orbit", "root", "root", "3306");
        }

        public void ConnectToDatabase(string server, string database, string uid, string password, string port)
        {
            string connectionString = $"SERVER={server};DATABASE={database};UID={uid};PASSWORD={password};PORT={port};Allow Zero Datetime=True;Convert Zero Datetime=True;";

            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                Debug.Log("Connected to database.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to connect to database: " + ex.Message);
            }
        }

        public void SaveGame(GameData data, int userId)
        {
            string query = "INSERT INTO game_data (user_id, game_time, max_health, max_mana, max_experience, current_health, current_mana, current_experience, level, player_position_x, player_position_y, player_position_z, chip) " +
                           "VALUES (@userId, @gameTime, @maxHealth, @maxMana, @maxExperience, @currentHealth, @currentMana, @currentExperience, @level, @posX, @posY, @posZ, @chip)";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@gameTime", data.gameTime);
                cmd.Parameters.AddWithValue("@maxHealth", data.maxHealth);
                cmd.Parameters.AddWithValue("@maxMana", data.maxMana);
                cmd.Parameters.AddWithValue("@maxExperience", data.maxExperience);
                cmd.Parameters.AddWithValue("@currentHealth", data.currentHealth);
                cmd.Parameters.AddWithValue("@currentMana", data.currentMana);
                cmd.Parameters.AddWithValue("@currentExperience", data.currentExperience);
                cmd.Parameters.AddWithValue("@level", data.level);
                cmd.Parameters.AddWithValue("@posX", data.playerPosition.x);
                cmd.Parameters.AddWithValue("@posY", data.playerPosition.y);
                cmd.Parameters.AddWithValue("@posZ", data.playerPosition.z);
                cmd.Parameters.AddWithValue("@chip", data.chip);

                try
                {
                    cmd.ExecuteNonQuery();
                    Debug.Log("Game saved successfully.");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed to save game: " + ex.Message);
                }
            }
        }

        public GameData LoadGame(int userId)
        {
            string query = "SELECT * FROM game_data WHERE user_id = @userId ORDER BY game_data_id DESC LIMIT 1";
            GameData data = null;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data = new GameData
                        {
                            gameTime = reader.GetFloat("game_time"),
                            maxHealth = reader.GetInt32("max_health"),
                            maxMana = reader.GetInt32("max_mana"),
                            maxExperience = reader.GetInt32("max_experience"),
                            currentHealth = reader.GetInt32("current_health"),
                            currentMana = reader.GetInt32("current_mana"),
                            currentExperience = reader.GetInt32("current_experience"),
                            level = reader.GetInt32("level"),
                            playerPosition = new Vector3(reader.GetFloat("player_position_x"), reader.GetFloat("player_position_y"), reader.GetFloat("player_position_z")),
                            chip = reader.GetInt32("chip")
                        };
                        Debug.Log("Game loaded successfully.");
                    }
                }
            }

            return data;
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
                            Debug.Log("Login successful.");
                            return true;
                        }
                    }
                }
            }

            Debug.LogError("Login failed.");
            return false;
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
                Debug.Log("Db 닫음");
            }
        }

        private void OnApplicationQuit()
        {
            CloseDBServer();
        }
    }
}