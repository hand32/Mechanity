using UnityEngine;
using System;

namespace roguelike
{

    [CreateAssetMenu(fileName = "New Android_Basic", menuName = "Monster/Android/Basic")]
    public class AndroidInfo_Basic : MonsterInfo
    {
        public float basicSmashRadius;
        public float damage_basicSmash;

        public float basicSmashPlayerCheckTime;
        public float basicSmashStartTime;
    }
}