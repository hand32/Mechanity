using UnityEngine;
using System;

namespace roguelike{


[CreateAssetMenu(fileName = "New Monster", menuName = "Monster/Monster")]
public class MonsterInfo : ScriptableObject {
	public new string name;
    public float findRadius;
    public float moveSpeed;
    
    public float damage_Charge;

}


}