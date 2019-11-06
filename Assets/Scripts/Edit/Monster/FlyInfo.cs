using UnityEngine;
using System;

namespace roguelike{

[CreateAssetMenu(fileName = "New Fly", menuName = "Monster/Fly")]
public class FlyInfo : MonsterInfo {
    public float rushSpeed;
	public float damage_Bomb;
    public float rushDelay;

}
}