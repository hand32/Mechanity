using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
namespace roguelike{
[CreateAssetMenu(fileName = "New Passive Item", menuName = "Item/Passive Item")]
public class PassiveItemInfo : ScriptableObject {
	public new string name;
	public Sprite sprite;
    [TextArea(3, 7)]
	public string details;

	public PassiveItemInfo()
	{
		name = "";
	}
}
}
