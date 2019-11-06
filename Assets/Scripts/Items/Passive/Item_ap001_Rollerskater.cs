using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap001_Rollerskater : MonoBehaviour
    {
        private CharacterPhysics m_playerPhysics;
        CharacterPhysics playerPhysics
        {
            get
            {
                if(m_playerPhysics == null)
                    m_playerPhysics = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterPhysics>();
                return m_playerPhysics;
            }
        }

        public float addSpeed = 1;

        bool isUsed;

        public void AddPlayerSpeed()
        {
            if(isUsed)
                return;
            playerPhysics.moveSpeed += addSpeed;
            isUsed = true;
        }

        public void MinusPlayerSpeed()
        {
            if(!isUsed)
                return;
            playerPhysics.moveSpeed -= addSpeed;
            isUsed = false;
        }

        void EquipThis()
        {
            AddPlayerSpeed();
        }

        void UnEquipThis()
        {
            MinusPlayerSpeed();
        }

    }
}