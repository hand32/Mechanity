using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_ap002_LightShoe : MonoBehaviour
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

        public void AddPlayerSpeed()
        {
            playerPhysics.m_jumpCount += 1;
        }

        public void MinusPlayerSpeed()
        {
            playerPhysics.m_jumpCount -= 1;
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