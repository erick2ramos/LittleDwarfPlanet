using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ldp
{
    public class PlanetController : MonoBehaviour
    {
        // Class to hold planet propertys like gravity
        public float Gravity;

        public GameObject obstaclePrefab;
        private GameObject[] obstacles;


        private Animator animator;
        private float randomPurposeTimer;
        private float radius;
        

        void Start()
        {
            animator = GetComponent<Animator>();
            radius = GetComponent<CircleCollider2D>().radius;

            obstacles = new GameObject[GameManager.Get.currentLevel];

            //Calculate random rotations for the obstacles around the planet
            for (int i = 0; i < obstacles.Length; i++)
            {
                float rotation = Random.Range(0, 359);
                obstacles[i] = Instantiate(obstaclePrefab, transform, false);
                obstacles[i].transform.rotation = Quaternion.Euler(0, 0, rotation);
            }
        }

        // Handling the animator
        // While the ship is close enough to the planet, the animator should be
        // in happy idle watching the ship state but if the velocity relative to
        // the planet is too much, the animator should be in alert.
        // If the ship crashes the state should be Ouch, on the other hand
        // if the ship lands safely in the planet the state would be win
        // Randomly from Idle state the planet could enter in laugh state

        private void Update()
        {
            if (randomPurposeTimer < Time.time && Random.Range(0, 100) <= 10)
            {
                randomPurposeTimer = Time.time + 10f;
                animator.SetTrigger("Laugh");
            }

            if(Vector2.Distance(transform.position, GameManager.Get.Player.transform.position) <= 3f * radius)
            {
                //Move eyes in the direction of the ship
                float playerVelocity = GameManager.Get.Player.GetComponent<Rigidbody2D>().velocity.sqrMagnitude;
                if (playerVelocity > 12f)
                {
                    animator.SetBool("Alert", true);
                } else
                {
                    animator.SetBool("Alert", false);
                }
            } else
            {
                animator.SetBool("Alert", false);
            }

            if (GameManager.Get.Player.state == ShipState.Crashed)
            {
                animator.SetTrigger("Lose");
            }

            if (GameManager.Get.Player.state == ShipState.Landed)
            {
                animator.SetTrigger("Win");
            }
        }
    }
}