using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap003_ReactiveArmor : MonoBehaviour
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

        public int addBaseHP = 15;
        public int addArmor = 3;

        public bool isUsed;

        void EquipThis()
        {
            StatusManagement.playerAfterGetDamageListeners.AddListener(PlusPlayerArmor);
            PlusPlayerHP();
        }

        void UnEquipThis()
        {
            StatusManagement.playerAfterGetDamageListeners.RemoveListener(PlusPlayerArmor);
            MinusPlayerHP();
        }

        public void PlusPlayerArmor(float _damage)
        {
            if(playerStatus.m_baseArmor < playerStatus.m_currentArmor + addArmor)
                playerStatus.m_currentArmor = playerStatus.m_baseArmor;
            else
                playerStatus.m_currentArmor += addArmor;
        }

        public void PlusPlayerHP()
        {
            if(isUsed)
                return;
            playerStatus.m_baseHealth += addBaseHP;
            isUsed = true;
        }
        public void MinusPlayerHP()
        {
            if(!isUsed)
                return;
            playerStatus.m_baseHealth -= addBaseHP;
            isUsed = false;
        }

    }
}