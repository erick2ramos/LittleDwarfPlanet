using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ldp
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Get
        {
            get
            {
                if(instance == null)
                {
                    GameObject go = new GameObject("MenuManager");
                    instance = go.AddComponent<MenuManager>();
                    instance.Init();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private static MenuManager instance;
        private Stack<GameObject> menus;
        private GameObject menuHolder;
        private GameObject canvas;

        public void Init()
        {
            menus = new Stack<GameObject>();
            canvas = GameObject.Find("Canvas");
            menuHolder = new GameObject("Menus");
            menuHolder.transform.SetParent(canvas.transform, false);
            RectTransform rt = menuHolder.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = Vector3.zero;
            rt.sizeDelta = Vector3.zero;
        }

        public GameObject Open(GameObject menuPrefab, bool isPopup = false)
        {
            if(menus.Count > 0 && !isPopup)
            {
                Destroy(menus.Pop());
            }
            GameObject go = Instantiate(menuPrefab, menuHolder.transform, false);
            menus.Push(go);
            return go;
        }

        public void Close()
        {
            Destroy(menus.Pop());
        }
    }
}