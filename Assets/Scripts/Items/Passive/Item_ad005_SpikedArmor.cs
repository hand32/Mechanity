using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ad005_SpikedArmor : MonoBehaviour
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
        
        public float returnRatio = 0.05f;

        void EquipThis()
        {
            StatusManagement.playerHitterListeners.AddListener(SendDamageToHitter);
        }

        void UnEquipThis()
        {
            StatusManagement.playerHitterListeners.RemoveListener(SendDamageToHitter);
        }

        public void SendDamageToHitter(GameObject hitter)
        {
            StatusManagement hitterStatus = hitter.GetComponent<StatusManagement>();
            if(hitterStatus != null)
                hitterStatus.GetDamage(Mathf.RoundToInt(playerStatus.m_baseHealth * returnRatio), playerStatus.gameObject);
                
        }

    }
}