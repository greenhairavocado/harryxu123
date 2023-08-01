using Managers;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDifficulty(int difficulty)
    {
        MainManager.Instance.difficulty = difficulty;
    }

    public void SignOutButton()
    {
        MainManager.Instance.authManager.SignOut();
    }

    public void DeleteUserButton()
    {
        MainManager.Instance.authManager.DeleteUser();
    }

    public void StartGameButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LandingTrial");
    }
}
