using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RK
{
    /// <summary>
    /// アイテムのデータベース
    /// inspectorでアイテムを登録し、idから該当のアイテムを認識できる
    /// </summary>
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;
        public WeaponItem unarmedWeapon;
        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();
        // ゲーム内の全アイテムのリスト
        [Header("Items")]
        private List<Item> items = new List<Item>();

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
            // すべての武器をアイテムリストに追加する
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }
            // すべてのアイテムに一意のアイテムIDを割り当てる
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }
        public WeaponItem GetWeaponByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
    }
}
