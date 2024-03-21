using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class UICamara : MonoBehaviour
    {
        public static UICamara instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);

        }
    }
}
