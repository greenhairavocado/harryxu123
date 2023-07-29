using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Handlers
{
    public class gamehandler : MonoBehaviour
    {
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI progressText;
        public Slider fuelSlider;
        public Slider progressSlider;
        public GameObject enemySlider;
        public GameObject rocketToggle;
        public GameObject joystick;
        public GameObject quizPanel;
        public LandingAI rocket;
        public GameObject opponent;

        float startTime;
        float currentFuel;

        [Header("Player Information")]
        public float progress;
        public int direction; // 0 is going up, 1 is going down
        public Transform initialGoal;
        public Transform platform;
        float initialDistance;
        public float questionProgressRequirement;

        private Vector3 storedPlayerVelocity;
        private Vector3 storedOpponentVelocity;
        private Vector3 storedPlayerAngularVelocity;
        private Vector3 storedOpponentAngularVelocity;

        private string correctAnswer;

        List<Tuple<string, string, string, string, string, string>> questions;

        // Start is called before the first frame update
        void Start()
        {
            questionProgressRequirement = 0.25f;
            startTime = Time.time;
            initialDistance = Vector3.Distance(rocket.transform.position, initialGoal.position);
            Debug.Log($"The initial distance is {initialDistance}");
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
            currentFuel = rocket.fuel;
            fuelSlider.value = currentFuel / 100;

            float t = Time.time - startTime;

            // 00:00:00
            // 120 vs 2:00
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2"); // --> 05

            timerText.text = minutes + ":" + seconds;

            if (direction == 0)
            {
                float currentDistance = Vector3.Distance(rocket.transform.position, initialGoal.position);
                // 0%-50% range
                progress = ((initialDistance - currentDistance) / initialDistance) / 2;
                progressText.text = $"{(int)(progress * 100)}%";

                if (progress >= 0.5)
                {
                    direction = 1;
                }
            }
            else
            {
                float currentDistance = Vector3.Distance(rocket.transform.position, platform.position);
                // 51%-100%
                progress = (((initialDistance - currentDistance) / initialDistance) / 2f) + 0.5f;
                progressText.text = $"{(int)(progress * 100)}%";
            }

            if (progress >= questionProgressRequirement)
            {
                questionProgressRequirement = questionProgressRequirement + 0.25f;
                // all rockets must be stopped
                PauseMovement();
                // Debug.Log($"Trigger: {progress} {questionProgressRequirement}");
                // question should be asked
                CreateQuizQuestion();

                
            }


            progressSlider.value = progress;
        }

        public void PauseMovement()
        {
            Rigidbody playerRigidbody = rocket.GetComponent<Rigidbody>();
            Rigidbody enemyRigidbody = opponent.GetComponent<Rigidbody>();

            storedPlayerVelocity = playerRigidbody.velocity;
            storedPlayerAngularVelocity = playerRigidbody.angularVelocity;

            storedOpponentVelocity = enemyRigidbody.velocity;
            storedOpponentAngularVelocity = enemyRigidbody.angularVelocity;

            playerRigidbody.isKinematic = true;
            enemyRigidbody.isKinematic = true;
        }

        public void ResumeMovement()
        {
            Rigidbody playerRigidbody = rocket.GetComponent<Rigidbody>();
            Rigidbody enemyRigidbody = opponent.GetComponent<Rigidbody>();

            playerRigidbody.isKinematic = false;
            enemyRigidbody.isKinematic = false;

            playerRigidbody.velocity = storedPlayerVelocity;
            playerRigidbody.angularVelocity = storedPlayerAngularVelocity;

            enemyRigidbody.velocity = storedOpponentVelocity;
            enemyRigidbody.angularVelocity = storedOpponentAngularVelocity;
        }

        public void CreateQuizQuestion()
        {
            enemySlider.SetActive(false);
            joystick.SetActive(false);
            rocketToggle.SetActive(false);

            quizPanel.SetActive(true);

            /*
            q1). What is the process by which a star exhausts its nuclear fuel and collapses under gravity?
            a) Supernova
            b) Black hole
            c) Red giant
            d) White dwarf

            q2). Which of the following is not a type of galaxy?
            a) Spiral
            b) Elliptical
            c) Irregular
            d) Nebula

            q3). What is the name of the phenomenon where light is bent as it passes through a gravitational field?
            a) Stellar parallax
            b) Redshift
            c) Gravitational lensing
            d) Cosmic microwave background
            */

            // pick a random question
            // var questions = new List<Tuple<string, string, string, string, string, string>>
            // {
            //     Tuple.Create("What is the process by which a star exhausts its nuclear fuel and collapses under gravity?", 
            //                  "Supernova", 
            //                  "Black Hole", 
            //                  "Red Giant", 
            //                  "White Dwarf",
            //                  "b"),
            //     Tuple.Create("Which of the following is not a type of galaxy?", 
            //                  "Spiral", 
            //                  "Elliptical", 
            //                  "Irregular", 
            //                  "Nebula",
            //                  "d"),
            //     Tuple.Create("What is the name of the phenomenon where light is bent as it passes through a gravitational field?", 
            //                  "Stellar parallax", 
            //                  "Redshift", 
            //                  "Gravitational Lensing", 
            //                  "Cosmic Microwave Background",
            //                  "c"),
            // };

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
            }
            else
            {
                Debug.Log("You got the question wrong!");
            }

            enemySlider.SetActive(true);
            joystick.SetActive(true);
            rocketToggle.SetActive(true);

            quizPanel.SetActive(false);
            ResumeMovement();
        }
    }

}
