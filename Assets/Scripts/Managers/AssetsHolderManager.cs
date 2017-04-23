using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ldp
{
    public class AssetsHolderManager : MonoBehaviour
    {
        // Singelton class to hold any asset needed by other manager or object
        // (last resort not much time left)

        public GameObject gameOverMenu;
        public AudioSource musicPlayer;
        public AudioSource sfxPlayer;
        public AudioClip musicClip;
        public AudioClip buttonClip;

        public static AssetsHolderManager Get
        {
            get { return instance; }
        }
        private static AssetsHolderManager instance;

        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else
            {
                Destroy(gameObject);
            }
        }

        public void ButtonClick()
        {
            sfxPlayer.PlayOneShot(buttonClip);
        }

        public void PlayMusic()
        {
            musicPlayer.clip = musicClip;
            musicPlayer.Play();
        }
    }
}