using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class Class_Summon : MonoBehaviour
    {
        [HideInInspector]
        public GameObject m_summon;
        public GameObject summonObject;

        public void AddSummon(GameObject item, GameObject _summon)
        {
            m_summon = Instantiate(_summon, GameObject.FindWithTag("Player").transform.position, Quaternion.Euler(0,0,0));
            Destroy(Class_SummonObject.summonList[ItemProgress.instance.itemEquipList.IndexOf(item)]);
            Class_SummonObject.summonList[ItemProgress.instance.itemEquipList.IndexOf(item)] = m_summon;
        }
        
        public void RemoveSummon()
        {
            Class_SummonObject.summonList[Class_SummonObject.summonList.IndexOf(m_summon)] = new GameObject();
            Destroy(m_summon);
        }
    }
}