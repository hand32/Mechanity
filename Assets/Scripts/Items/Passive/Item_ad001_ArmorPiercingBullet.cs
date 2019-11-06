using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ad001_ArmorPiercingBullet : MonoBehaviour
    {
        private GameObject m_playerObject;
        GameObject playerObject
        {
            get
            {
                if (m_playerObject == null)
                    m_playerObject = GameObject.FindGameObjectWithTag("Player");
                return m_playerObject;
            }
        }

        public int addWeaponDamage = 3;

        void EquipThis()
        {
            Class_Weapon.FireBulletListener.AddListener(AddPenetrate);
            Class_Weapon.WeaponPickListener.AddListener(AddWeaponDamage);
            Class_Weapon.WeaponDropListener.AddListener(ReduceWeaponDamage);
            AddWeaponDamage(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        void UnEquipThis()
        {
            Class_Weapon.FireBulletListener.RemoveListener(AddPenetrate);
            Class_Weapon.WeaponPickListener.RemoveListener(AddWeaponDamage);
            Class_Weapon.WeaponDropListener.RemoveListener(ReduceWeaponDamage);
            ReduceWeaponDamage(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        public void AddPenetrate(GameObject _bullet)
        {
            _bullet.GetComponent<HitCheck>().penetrateCount ++;
        }

        public void AddWeaponDamage(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.damage += addWeaponDamage;
            }
        }

        public void ReduceWeaponDamage(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.damage -= addWeaponDamage;
            }
        }

    }
}