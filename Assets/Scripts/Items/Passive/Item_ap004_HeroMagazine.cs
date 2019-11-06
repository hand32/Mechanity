using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap004_HeroMagazine : MonoBehaviour
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

        public float addMagazineRatio = 0.1f;

        Class_Weapon weapon;

        void EquipThis()
        {
            Class_Weapon.WeaponPickListener.AddListener(ExtendMagazine);
            Class_Weapon.WeaponDropListener.AddListener(ReduceMagazine);
            Class_Weapon.WeaponFireListener.AddListener(DontUseAmmo);
            ExtendMagazine(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        void UnEquipThis()
        {
            Class_Weapon.WeaponPickListener.RemoveListener(ExtendMagazine);
            Class_Weapon.WeaponDropListener.RemoveListener(ReduceMagazine);
            Class_Weapon.WeaponFireListener.RemoveListener(DontUseAmmo);
            ReduceMagazine(playerObject.GetComponentInChildren<Class_Weapon>());
        }

        public void ExtendMagazine(Class_Weapon _weapon)
        {
            weapon = _weapon;
            if (_weapon != null)
            {
                preAmmo = _weapon.originalWeaponInfo.ammo;
                _weapon.weaponInfo.ammo += Mathf.RoundToInt(preAmmo * addMagazineRatio);
            }
        }

        public void ReduceMagazine(Class_Weapon _weapon)
        {
            if (_weapon != null)
            {
                _weapon.weaponInfo.ammo -= Mathf.RoundToInt(preAmmo * addMagazineRatio);
                if(_weapon.ammo > _weapon.weaponInfo.ammo)
                    _weapon.ammo = _weapon.weaponInfo.ammo;
            }
        }

        public void DontUseAmmo(GameObject _weapon)
        {
            if(weapon != null)
            {
                if(Random.Range(0, 1f) <= 0.5f / weapon.weaponInfo.fireRate)
                {
                    if(weapon.ammo < weapon.weaponInfo.ammo)
                        weapon.ammo += 1;
                }
            }
        }

    }
}