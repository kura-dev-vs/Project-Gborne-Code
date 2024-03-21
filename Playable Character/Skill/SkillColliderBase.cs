using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// スキルコライダーのベース
    /// 今は生成されるエネルギーのみ書いてあるが将来的に拡張
    /// </summary>
    public class SkillColliderBase : MonoBehaviour
    {
        [Header("Soul Modifiers")]
        public float physicalDamage;
        public float magicDamage;
        public float fireDamage;
        public float holyDamage;
        public GameObject energyObject;
        bool generatedFlag = false;
        protected virtual void GenerateEnergy(int energy, EntryManager entry, Vector3 contactPoint)
        {
            if (!generatedFlag)
            {
                for (int i = 0; i < energy; i++)
                {
                    var generated = Instantiate(energyObject, contactPoint, Quaternion.identity);
                    generated.GetComponent<EnergyManager>().InstantiationEnergy(entry);
                }
                generatedFlag = true;
            }

        }
    }
}
