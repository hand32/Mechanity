using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap005_LuckyCoin : MonoBehaviour
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
        

        [Range(0, 100)]
        public float damageReduceChance = 1f/2f * 100;
        
        void EquipThis()
        {
            StatusManagement.playerDamagedModifyerList.Add(RandomDamageModify);
        }

        void UnEquipThis()
        {
            StatusManagement.playerDamagedModifyerList.Remove(RandomDamageModify);
        }

        public float RandomDamageModify(float _damage)
        {
            if(Random.Range(0, 1f) <= damageReduceChance / 100f)
                _damage = Mathf.RoundToInt(_damage * 0.5f);
            else
                _damage = Mathf.RoundToInt(_damage * 1.5f);
                
            return _damage;
        }

    }
}