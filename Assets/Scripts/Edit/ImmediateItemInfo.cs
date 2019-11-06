using UnityEngine;
namespace roguelike{
[CreateAssetMenu(fileName = "New Immediate Item", menuName = "Item/Immediate Item")]
public class ImmediateItemInfo : ScriptableObject {
	public new string name;
	public Sprite Sprite;
	public float hpHeal;
	public float armorHeal;

}
}
