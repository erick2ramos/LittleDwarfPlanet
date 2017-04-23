﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lsp
{
    public class BreadcrumbsController : MonoBehaviour
    {
        public float maxLifeTime;
        private float lifeTimer;

        // Use this for initialization
        void Start()
        {
            lifeTimer = maxLifeTime;
            transform.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            lifeTimer = maxLifeTime;
            transform.localScale = Vector3.one;
        }

        // Update is called once per frame
        void Update()
        {
            lifeTimer -= Time.deltaTime;
            transform.localScale = Vector3.one * (lifeTimer / maxLifeTime);
            if (lifeTimer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}