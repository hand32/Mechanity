using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap009_ThirdLevelHelmet : MonoBehaviour
    {
        private StatusManagement m_PlayerStatus;

        StatusManagement playerStatus
        {
            get
            {
                if(m_PlayerStatus == null)
                    m_PlayerStatus = GameObject.FindGameObjectWithTag("Player").GetComponent<StatusManagement>();
                return m_PlayerStatus;
            }
        }

        public float damageFixRatio = 0.1f;

        void EquipThis()
        {
            StatusManagement.playerDamagedModifyerList.Add(DamageReduceCheck);
        }

        void UnEquipThis()
        {
            StatusManagement.playerDamagedModifyerList.Remove(DamageReduceCheck);
        }

        public float DamageReduceCheck(float _damage)
        {
            if(_damage > playerStatus.m_baseHealth * damageFixRatio)
                _damage = Mathf.RoundToInt(playerStatus.m_baseHealth * damageFixRatio);
                
            return _damage;
        }

    }
}