using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    public class FreeLookCameraCustomEvent : MonoBehaviour
    {
        public void ResetLocalRotaiton()
        {
            Quaternion rot = default;
            rot.w = 1f;
            Transform transform = gameObject.GetComponent<Transform>();
            transform.localRotation = rot;
        }
    }
}
