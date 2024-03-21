using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 武器モデルをinstantiateするためのスクリプト
    /// このコンポーネントを持つゲームオブジェクトの位置に武器が生成される
    /// </summary>
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        public WeaponModelSlot weaponSlot;
        public GameObject currentWeaponModel;
        public void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }
        public void LoadWeapon(GameObject weaponModel)
        {
            currentWeaponModel = weaponModel;
            weaponModel.transform.parent = transform;

            weaponModel.transform.localPosition = Vector3.zero;
            weaponModel.transform.localRotation = Quaternion.identity;
            //weaponModel.transform.localScale = Vector3.one;
        }
    }
}
