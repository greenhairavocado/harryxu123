 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            Debug.Log("Firebase Initialized");
        }

        void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (auth.CurrentUser != user)
            {
                bool loggedIn = (user != auth.CurrentUser && auth.CurrentUser != null);

                if (!loggedIn && user != null)
                {
                    Debug.Log("user logged out");

                    // TODO more logout stuff
                    SceneManager.LoadScene("Login");
                }

                user = auth.CurrentUser;

                if (loggedIn)
                {
                    Debug.Log("user logged in");
                    // go to main menu
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }

        public IEnumerator SignUp(string email, string password, string name)
        {
            var signupTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => signupTask.IsCompleted);

            if (signupTask.Exception == null)
            {
                Debug.Log(signupTask.Result);

                // set the display name of the firebase user
                var profile = new Firebase.Auth.UserProfile
                {
                    DisplayName = name,
                };

                var profileTask = user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                // user = signupTask.user;
                // if (user != null)
                // {
                //     // do post-signup stuff here
                // }
            }
        }

        public IEnumerator Login(string email, string password)
        {
            Debug.Log("Attempting to login");
            var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

            if (loginTask.Exception == null)
            {
                Debug.Log("User logged in successfully");
                // user = loginTask.user;
                Debug.Log(loginTask.Result);

                // do other stuff here
            }
        }

        public void SignOut()
        {
            // MainManager.Instance
            auth.SignOut();

        }

        public void DeleteUser()
        {
            StartCoroutine(DeleteUserAsync());
        }

        IEnumerator DeleteUserAsync()
        {
            var deleteTask = user.DeleteAsync();
            yield return new WaitUntil(predicate: () => deleteTask.IsCompleted);

            if (deleteTask.Exception == null)
            {
                Debug.Log("User deleted successfully");

                // logout
                auth.SignOut();
            }
        }
    }

}
