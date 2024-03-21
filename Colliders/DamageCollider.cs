using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using RK;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;

/// <summary>
/// ダメージを与えるコライダー ("受ける"ではなく"与える"。武器の当たり判定などに使用) 
/// </summary>
public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;
    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;
    [Header("Contact Point")]
    protected Vector3 contactPoint;
    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {

    }
    /// <summary>
    /// ダメージコライダーに入ってきた別colliderがcharacterオブジェクトか調べ、そうである場合は対象とし処理を行う (同じグループなら無視とか、相手がガード状態であるとか)
    /// </summary>
    /// <param name="other"></param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }
    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        // 同じ相手にダメージを二度与えたくないため、一度与えた相手はlistに加えてそれ以降スルーする (colliderを閉じる際にクリアされる)
        if (charactersDamaged.Contains(damageTarget))
            return;
        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contactPoint;

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }
    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public virtual void DisableDamageCollider()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
        // listのリセット
        charactersDamaged.Clear();
    }
}
