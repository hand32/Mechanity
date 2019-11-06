using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap007_QuickDrawMagazine : MonoBehaviour
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

        public float reloadTimeReduce = 0.2f;

        void EquipThis()
        {
            Class_Weapon.WeaponPickListener.AddListener(QuickReloadMagazine);
            Class_Weapon.WeaponDropListener.AddListener(ResetReloadMagazine);
            QuickReloadMagazine(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        void UnEquipThis()
        {
            Class_Weapon.WeaponPickListener.RemoveListener(QuickReloadMagazine);
            Class_Weapon.WeaponDropListener.RemoveListener(ResetReloadMagazine);
            ResetReloadMagazine(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        public void QuickReloadMagazine(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.relodeSpeed -= reloadTimeReduce;
            }
        }

        public void ResetReloadMagazine(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.relodeSpeed += reloadTimeReduce;
            }
        }

    }
}