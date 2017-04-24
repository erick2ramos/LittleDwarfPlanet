using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ldp
{
    public enum ShipState
    {
        Alive,
        Crashed,
        NoFuel,
        Landed
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipController : MonoBehaviour
    {
        public float maxFuelQty;
        public float burnSpeed;
        public float fuelConsumtionRate;
        public float currentFuelQty;

        public AudioSource shipSfx;
        public AudioClip shipCrash;
        public AudioClip shipLanded;
        public AudioClip shipEngine;
        public AudioClip shipTouchDown;

        public GameObject fire;
        public ShipState state;

        public PlanetController targetPlanet;

        //Breadcrumbs
        public GameObject breadcrumbsPrefab;
        public int maxBreadcrumbs;
        public float breadcrumbCooldown;
        private float breadcrumbTimer;
        private int currentBreadcrumb;

        private GameObject[] breadcrumbs;
        private Rigidbody2D Rigidbody;
        private Transform gravityCenter;
        private Vector2 initialVelocity;

        // Landing attributes
        private float landingTimer;
        private bool playingSound;

        void Start()
        {
            breadcrumbs = new GameObject[maxBreadcrumbs];
            GameObject holder = new GameObject("BCHolder");
            for (int i = 0; i < maxBreadcrumbs; i++)
            {
                breadcrumbs[i] = Instantiate(breadcrumbsPrefab, holder.transform, false);
                breadcrumbs[i].SetActive(false);
            }
            currentFuelQty = maxFuelQty;
            gravityCenter = targetPlanet.GetComponent<Transform>();
            Rigidbody = GetComponent<Rigidbody2D>();
            state = ShipState.Alive;
            landingTimer = float.MaxValue;

            // Ship starts on perfect circle orbit around center
            // Calculation to get the initial velocity 
            Vector2 centerOfGravity = gravityCenter.position;
            Vector2 shipPosition = transform.position;
            Vector2 planetVector = centerOfGravity - shipPosition;
            Vector2 startingOrientation = Vector3.Cross(planetVector, transform.forward).normalized;
            float rand = Random.Range(0, 10);
            if (rand <= 5f)
                startingOrientation = startingOrientation  * -1;

            float startingSpeed = planetVector.magnitude / (targetPlanet.Gravity);
            initialVelocity = startingOrientation * startingSpeed;
            transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(startingOrientation.y, startingOrientation.x));
        }

        void Update()
        {
            if(GameManager.Get.isPaused || GameManager.Get.isGameOver) { return; }
            Vector2 finalVelocity = initialVelocity;
            initialVelocity = Vector2.zero;
            // Controls for handling ship

            // Fuel burn: on button use fuel to steer the ship

            // Can rotate ship in z-axis, expending some fuel
            // "D" rotate clockwise
            // "A" rotate counter-clockwise
            if (Input.GetAxis("Horizontal") != 0 && currentFuelQty > 0)
            {
                Rigidbody.AddTorque( -Input.GetAxis("Horizontal"));
                currentFuelQty = Mathf.Max(currentFuelQty - fuelConsumtionRate * 0.1f * Time.deltaTime, 0);
            }

            // On "Space" do sustained fuel burn to move ship forward (upwards of the ship)
            if (Input.GetButton("Jump") && currentFuelQty > 0)
            {
                if (!playingSound)
                {
                    playingSound = true;
                    shipSfx.loop = true;
                    shipSfx.clip = shipEngine;
                    shipSfx.Play();
                }
                Vector2 burnOrientation = transform.up;
                currentFuelQty = Mathf.Max(currentFuelQty - fuelConsumtionRate * Time.deltaTime, 0);
                finalVelocity += burnOrientation * burnSpeed / 2;
                Debug.DrawRay(transform.position, transform.up, Color.red);
                fire.SetActive(true);
            }
            else
            {
                if (playingSound)
                {
                    playingSound = false;
                    shipSfx.loop = false;
                    shipSfx.Stop();
                }

                fire.SetActive(false);
            }
            Vector2 planetVector = gravityCenter.position - transform.position;
            // If fuel reserves depletes you are in big trouble, so game over
            if (currentFuelQty <= 0 || (gravityCenter.position - transform.position).magnitude > 100f)
            {
                state = ShipState.NoFuel;
                GameManager.Get.GameOver();
            }

            if(breadcrumbTimer <= Time.time && !GameManager.Get.isGameOver)
            {
                breadcrumbTimer = Time.time + breadcrumbCooldown;
                Vector2 newPos = transform.position;
                breadcrumbs[currentBreadcrumb].transform.position = newPos;
                breadcrumbs[currentBreadcrumb].SetActive(true);
                currentBreadcrumb = (currentBreadcrumb + 1) % maxBreadcrumbs;
            }

            // Ship is allways falling towards a transform position
            finalVelocity += planetVector.normalized * targetPlanet.Gravity * Time.deltaTime / 2;
            Rigidbody.velocity += finalVelocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            shipSfx.PlayOneShot(shipTouchDown);
            // if collision is done with ship legs and velocity is tolerable then ship can land
            // the right vector of the transform should be (or almost be) orthogonal to the
            // final velocity
            if (collision.collider.gameObject.tag == "Planet" &&
                collision.relativeVelocity.sqrMagnitude < 12f)
            {
                // Soft landing?
                landingTimer = Time.time + 2.5f;
            } else
            {
                Die();
                GameManager.Get.GameOver();
            }
            Debug.Log(collision.relativeVelocity.sqrMagnitude);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            // Check for a timer to finalize run as ship did land safely enough
            float orthoVector = Vector2.Dot(Rigidbody.velocity.normalized, transform.right);
            if (collision.collider.gameObject.tag == "Planet" && 
                orthoVector <= 0.1f && orthoVector >= -0.1f &&
                Time.time > landingTimer)
            {
                state = ShipState.Landed;
                shipSfx.PlayOneShot(shipLanded);
                GameManager.Get.GameOver();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // if ship triggers/collides with other thing rather than legs then ship crashed
            Die();
            GameManager.Get.GameOver();
        }

        public void Die()
        {
            // Function should handle any kind of dieing animation
            shipSfx.loop = false;
            shipSfx.Stop();
            state = ShipState.Crashed;
            shipSfx.PlayOneShot(shipCrash);
            Rigidbody.velocity = Vector2.zero;
        }
    }
}