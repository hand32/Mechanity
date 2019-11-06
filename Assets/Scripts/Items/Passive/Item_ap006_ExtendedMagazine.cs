using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap006_ExtendedMagazine : MonoBehaviour
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
        int preAmmo;

        public float addAmmoRatio = 0.5f;

        void EquipThis()
        {
            Class_Weapon.WeaponPickListener.AddListener(ExtendMagazine);
            Class_Weapon.WeaponDropListener.AddListener(ReduceMagazine);
            ExtendMagazine(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        void UnEquipThis()
        {
            Class_Weapon.WeaponPickListener.RemoveListener(ExtendMagazine);
            Class_Weapon.WeaponDropListener.RemoveListener(ReduceMagazine);
            ReduceMagazine(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        public void ExtendMagazine(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                preAmmo = _weapon.originalWeaponInfo.ammo;
                _weapon.weaponInfo.ammo += Mathf.RoundToInt(preAmmo * addAmmoRatio);
            }
        }

        public void ReduceMagazine(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.ammo -= Mathf.RoundToInt(preAmmo * addAmmoRatio);
                if(_weapon.ammo > _weapon.weaponInfo.ammo)
                    _weapon.ammo = _weapon.weaponInfo.ammo;
            }
        }

    }
}