using UnityEngine;

namespace NOLDA
{
    [CreateAssetMenu(fileName = "DBConfig", menuName = "NOLDA/DBConfig", order = 1)]
    public class DBConfigSO : ScriptableObject
    {
        [Header("Database Server Settings")]
        public string serverIP = "localhost";
        public string port = "3306";
        public string host = "root";
        public string password = "root";
    }
}

