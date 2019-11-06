using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ad008_ResoluteOneShot : MonoBehaviour
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

        public int addDamageAtChance = 20;

        public int addWeaponDamage = 5;

        void EquipThis()
        {
            Class_Weapon.FireBulletListener.AddListener(AddDamage);
            Class_Weapon.WeaponPickListener.AddListener(AddWeaponDamage);
            Class_Weapon.WeaponDropListener.AddListener(ReduceWeaponDamage);
            AddWeaponDamage(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        void UnEquipThis()
        {
            Class_Weapon.FireBulletListener.RemoveListener(AddDamage);
            Class_Weapon.WeaponPickListener.RemoveListener(AddWeaponDamage);
            Class_Weapon.WeaponDropListener.RemoveListener(ReduceWeaponDamage);
            ReduceWeaponDamage(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        public void AddDamage(GameObject _bullet)
        {
            if(Random.Range(0, 1f) <= 1/playerObject.GetComponentInChildren<Class_Weapon>().weaponInfo.fireRate)
                _bullet.GetComponent<HitCheck>().damage += addDamageAtChance;
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