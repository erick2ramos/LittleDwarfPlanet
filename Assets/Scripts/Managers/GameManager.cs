using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ldp
{
    public class GameManager : MonoBehaviour
    {

        public ShipController Player;
        public PlanetController TargetPlanet;

        public ScoreHolder Score;
        public bool isPaused;
        public bool isGameOver;

        //Play timer countdown
        //Player has X seconds to land ship on planet
        public float playTimer;
        public float startTimer;

        public static bool isLoaded = false;
        public static GameManager Get
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    isLoaded = true;
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private static GameManager instance = null;

        public void NewGame()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipController>();
            TargetPlanet = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetController>();
            playTimer = 120f;
            startTimer = 2.5f;
            Score = new ScoreHolder(Vector2.Distance(Player.transform.position, TargetPlanet.transform.position), playTimer);
            isPaused = false;
            Time.timeScale = 1;
            isGameOver = false;
        }

        private void Update()
        {
            if (!isPaused && !isGameOver)
            {
                if (startTimer > 0)
                {
                    // Start timer gives a chance for the player to "syncronize" with
                    // controller and other stuff so he can
                    startTimer -= Time.deltaTime;
                } else
                {
                    if (playTimer <= 0)
                    {
                        GameOver();
                    }
                    else
                    {
                        playTimer = Mathf.Max(0, playTimer - Time.deltaTime);
                        Score.timeLeft = playTimer;
                        Score.SetMaxDistanceTowardsPlanet(Player.transform, TargetPlanet.transform);
                    }
                }
            }
        }

        public void Pause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }

        public void GameOver()
        {
            if (isGameOver) { return; }
            // Has the job to show on screen the final score and give options to
            // replay or quit game
            isGameOver = true;
            GameObject menu = MenuManager.Get.Open(AssetsHolderManager.Get.gameOverMenu);
            string finalMsg = "";
            bool alive = false;
            switch (Player.state)
            {
                case ShipState.Landed:
                    {
                        finalMsg = "Congratulations";
                        alive = true;
                        break;
                    }
                case ShipState.NoFuel:
                    {
                        finalMsg = "No Fuel Left";
                        break;
                    }
                case ShipState.Crashed:
                    {
                        finalMsg = "You Crashed";
                        break;
                    }
                default:
                    {
                        finalMsg = "Game Over";
                        break;
                    }
            }
            string finalScore = Score.FinalScore(alive).ToString("00000");

            menu.GetComponent<GameoverMenuController>().finalScore.text = finalScore;
            menu.GetComponent<GameoverMenuController>().gameMsg.text = finalMsg;
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}