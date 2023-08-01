using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;
        public AuthManager authManager;
        public int difficulty = 0;

        void Awake()
        {
            Instance = this;

            authManager = GetComponent<AuthManager>();
        }
    }
}

