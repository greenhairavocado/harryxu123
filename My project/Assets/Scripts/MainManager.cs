using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance;
        public AuthManager authManager;
        void Awake()
        {
            Instance = this;

            authManager = GetComponent<AuthManager>();
        }
    }
}

