using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ad006_ShockedBullet : MonoBehaviour
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
        public int addDamage = 7;

        void EquipThis()
        {
            Class_Weapon.WeaponPickListener.AddListener(AddDamage);
            Class_Weapon.WeaponDropListener.AddListener(ReduceDamage);
            AddDamage(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        void UnEquipThis()
        {
            Class_Weapon.WeaponPickListener.RemoveListener(AddDamage);
            Class_Weapon.WeaponDropListener.RemoveListener(ReduceDamage);
            ReduceDamage(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        public void AddDamage(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.damage += addDamage;
            }
        }

        public void ReduceDamage(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.damage -= addDamage;
            }
        }

    }
}