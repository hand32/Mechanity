using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;


namespace roguelike
{
    public class Item_su004_FloatingShield : Class_Summon
    {

        void EquipThis()
        {
            AddSummon(gameObject, summonObject);
        }

        void UnEquipThis()
        {
            RemoveSummon();
        }

    }
}