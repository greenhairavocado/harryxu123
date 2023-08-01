using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Handlers
{
    public class gamehandler : MonoBehaviour
    {
        [Header("Panel References")]
        public GameObject restartPanel;
        public GameObject quizPanel;
        public GameObject resultPanel;

        [Header("UI References")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI progressText;
        public TextMeshProUGUI opponentProgressText;
        public Slider fuelSlider;
        public Slider progressSlider;
        public GameObject enemySliderObject;
        Slider opponentSlider;
        public GameObject rocketToggle;
        public GameObject joystick;
        
        [Header("Object References")]
        public LandingAI rocket;
        public GameObject opponent;
        public List<Vector3> checkpoints;
        [HideInInspector]
        public int lastCheckpoint = -1;

        float startTime;
        float currentFuel;

        [Header("Player Information")]
        public float progress;
        public int direction; // 0 is going up, 1 is going down
        public int opponentDirection;
        public Transform initialGoal;
        public Transform platform;
        float initialDistance;
        float initialOpponentDistance;
        public float questionProgressRequirement;

        private Vector3 storedPlayerVelocity;
        private Vector3 storedOpponentVelocity;
        private Vector3 storedPlayerAngularVelocity;
        private Vector3 storedOpponentAngularVelocity;

        private string correctAnswer;
        [HideInInspector]
        public bool isQuizActive;
        [HideInInspector]
        public bool playerIsDone;
        float playerTime;
        [HideInInspector]
        public bool opponentIsDone;
        float opponentTime;

        List<Tuple<string, string, string, string, string, string>> questions;

        // Start is called before the first frame update
        void Start()
        {
            opponentSlider = enemySliderObject.GetComponent<Slider>();
            questionProgressRequirement = 0.25f;
            startTime = Time.time;
            initialDistance = Vector3.Distance(rocket.transform.position, initialGoal.position);
            initialOpponentDistance = Vector3.Distance(opponent.transform.position, initialGoal.position);
            // Debug.Log($"The initial distance is {initialDistance}");
            // consider velocity if applicable

            questions = new List<Tuple<string, string, string, string, string, string>>
            {
                Tuple.Create("What is the process by which a star exhausts its nuclear fuel and collapses under gravity?", 
                            "Supernova", 
                            "Black Hole", 
                            "Red Giant", 
                            "White Dwarf",
                            "b"),
                Tuple.Create("Which of the following is not a type of galaxy?", 
                            "Spiral", 
                            "Elliptical", 
                            "Irregular", 
                            "Nebula",
                            "d"),
                Tuple.Create("What is the name of the phenomenon where light is bent as it passes through a gravitational field?", 
                            "Stellar parallax", 
                            "Redshift", 
                            "Gravitational Lensing", 
                            "Cosmic Microwave Background",
                            "c"),
            };
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log(direction);
            currentFuel = rocket.fuel;
            fuelSlider.value = currentFuel / 100;

            float t = Time.time - startTime;

            // // 00:00:00
            // // 120 vs 2:00
            // string minutes = ((int)t / 60).ToString();
            // string seconds = (t % 60).ToString("f2"); // --> 05

            timerText.text = FormatTime(t);
            UpdateAIProgress();

            if (direction == 0)
            {
                float currentDistance = Vector3.Distance(rocket.transform.position, initialGoal.position);
                // 0%-50% range
                progress = ((initialDistance - currentDistance) / initialDistance) / 2;
                progressText.text = $"{(int)(progress * 100)}%";

                // if (progress >= 0.5)
                // {
                //     direction = 1;
                // }
            }
            else
            {
                float currentDistance = Vector3.Distance(rocket.transform.position, platform.position);
                // 51%-100%
                progress = (((initialDistance - currentDistance) / initialDistance) / 2f) + 0.5f;
                progressText.text = $"{(int)(progress * 100)}%";
            }

            if (progress >= questionProgressRequirement && questionProgressRequirement != 0.5f)
            {
                questionProgressRequirement = questionProgressRequirement + 0.25f;
                lastCheckpoint += 1;
                // all rockets must be stopped
                PauseMovement();
                // Debug.Log($"Trigger: {progress} {questionProgressRequirement}");
                // question should be asked
                CreateQuizQuestion();
            }


            progressSlider.value = progress;
            if (playerIsDone)
            {
                restartPanel.SetActive(false);
                progressSlider.value = 1;
                progressText.text = "100%";
            }

            if (opponentIsDone)
            {
                opponentSlider.value = 1;
                opponentProgressText.text = "100%";
            }


            if (playerIsDone && opponentIsDone)
            {
                if (opponentTime < playerTime)
                {
                    resultPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lose!";
                }
                else
                {
                    resultPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Win!";
                }

                // write both your time and opp time in the result panel with 00:00:00 format
                resultPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Your Time: {FormatTime(playerTime)}\nOpp Time: {FormatTime(opponentTime)}";

                resultPanel.SetActive(true);
            }
        }

        void UpdateAIProgress()
        {
            if (opponentDirection == 0)
            {
                float currentDistance = Vector3.Distance(opponent.transform.position, initialGoal.position);
                // 0%-50% range
                opponentSlider.value = ((initialOpponentDistance - currentDistance) / initialOpponentDistance) / 2;
                opponentProgressText.text = $"{(int)(opponentSlider.value * 100)}%";
            }
            else
            {
                float currentDistance = Vector3.Distance(opponent.transform.position, platform.position);
                // 51%-100%
                opponentSlider.value = (((initialOpponentDistance - currentDistance) / initialOpponentDistance) / 2f) + 0.5f;
                opponentProgressText.text = $"{(int)(opponentSlider.value * 100)}%";
            }
        }

        string FormatTime(float time)
        {
            int intTime = (int)time;
            int minutes = intTime / 60;
            int seconds = intTime % 60;
            float fraction = time;
            fraction = (fraction % 1) * 100;
            int milliseconds = (int)fraction;

            string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, milliseconds);

            return timeText;
        }

        public void PauseMovement(bool playerOnly=false)
        {
            Rigidbody playerRigidbody = rocket.GetComponent<Rigidbody>();
            
            storedPlayerVelocity = playerRigidbody.velocity;
            storedPlayerAngularVelocity = playerRigidbody.angularVelocity;
            playerRigidbody.isKinematic = true;

            if (!playerOnly)
            {
                Rigidbody enemyRigidbody = opponent.GetComponent<Rigidbody>();
                storedOpponentVelocity = enemyRigidbody.velocity;
                storedOpponentAngularVelocity = enemyRigidbody.angularVelocity;
                enemyRigidbody.isKinematic = true;
            }
        }

        public void ResumeMovement(bool playerOnly=false, bool reset=false)
        {
            Rigidbody playerRigidbody = rocket.GetComponent<Rigidbody>();

            playerRigidbody.isKinematic = false;

            if (!reset)
            {
                playerRigidbody.velocity = storedPlayerVelocity;
                playerRigidbody.angularVelocity = storedPlayerAngularVelocity;
            }

            if (!playerOnly)
            {
                Rigidbody enemyRigidbody = opponent.GetComponent<Rigidbody>();
                enemyRigidbody.isKinematic = false;

                enemyRigidbody.velocity = storedOpponentVelocity;
                enemyRigidbody.angularVelocity = storedOpponentAngularVelocity;
            }
        }

        public void CreateQuizQuestion()
        {
            isQuizActive = true;
            enemySliderObject.SetActive(false);
            joystick.SetActive(false);
            rocketToggle.SetActive(false);

            quizPanel.SetActive(true);

            var randomIndex = UnityEngine.Random.Range(0, questions.Count);
            // Debug.Log($"{randomIndex} {questions.Count}");
            var selectedQuestion = questions[randomIndex];
            questions.RemoveAt(randomIndex);

            quizPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = selectedQuestion.Item1;

            quizPanel.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"A. {selectedQuestion.Item2}";
            quizPanel.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"B. {selectedQuestion.Item3}";
            quizPanel.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"C. {selectedQuestion.Item4}";
            quizPanel.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"D. {selectedQuestion.Item5}";

            correctAnswer = selectedQuestion.Item6;
        }

        public void QuestionChoice(string choice)
        {
            if (choice == correctAnswer)
            {
                Debug.Log("You got the question right!");
                rocket.fuel = 100;
            }
            else
            {
                Debug.Log("You got the question wrong!");
                rocket.fuel -= 10;
            }

            if (questionProgressRequirement == 0.75f)
            {
                direction = 1;
            }

            isQuizActive = false;

            enemySliderObject.SetActive(true);
            joystick.SetActive(true);
            rocketToggle.SetActive(true);

            quizPanel.SetActive(false);
            ResumeMovement();
        }

        public void RequestRestart(string reason)
        {
            // if (playerIsDone) return;
            PauseMovement(true);
            restartPanel.SetActive(true);

            restartPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = reason;
        }

        public void Restart()
        {
            ResumeMovement(true, true);
            if (lastCheckpoint == -1)
            {
                rocket.GetComponent<LandingAI>().RestartAgent();
            }
            else
            {
                rocket.transform.position = checkpoints[lastCheckpoint];
            }

        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void IndicateCompletion(bool isPlayer)
        {
            if (isPlayer)
            {
                playerIsDone = true;
                playerTime = Time.time - startTime;
            }
            else
            {
                opponentIsDone = true;
                opponentTime = Time.time - startTime;
            }
        }
    }

}
