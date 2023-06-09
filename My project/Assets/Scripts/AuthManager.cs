 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

namespace Managers
{
    public class AuthManager : MonoBehaviour
    {
        public FirebaseAuth auth;
        public FirebaseUser user;

        // Start is called before the first frame update
        void Start()
        {
            InitializeFirebase();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void InitializeFirebase()
        {
            auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            Debug.Log("Firebase Initialized");
        }

        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            // if (auth.currentUser != user)
            // {
            //     bool loggedIn = (user != auth.CurrentUser && auth.CurrentUser != null);

            //     if (!loggedIn && user != null)
            //     {
            //         Debug.Log("user logged out");

            //         // TODO more logout stuff
            //     }

            //     user = auth.currentUser;

            //     if (loggedIn)
            //     {
            //         Debug.Log("user logged in");
            //     }
            // }
        }

        // public IEnumerator SignUp(string email, string password)
        // {
        //     // var signupTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        //     // yield return new WaitUntil(predicate: () => signupTask.IsCompleted);

        //     // if (signupTask.Exception == null)
        //     // {
        //     //     user = signupTask.Result;
        //     //     if (user != null)
        //     //     {
        //     //         // do post-signup stuff here
        //     //     }
        //     // }
        // }

        // public IEnumerator Login(string email, string password)
        // {
        //     // var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        //     // yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        //     // if (loginTask.Exception == null)
        //     // {
        //     //     Debug.Log("User logged in successfully");
        //     //     user = loginTask.Result;

        //     //     // do other stuff here
        //     // }
        // }

        public void SignOut()
        {
            // MainManager.Instance
            auth.SignOut();

        }
    }

}
