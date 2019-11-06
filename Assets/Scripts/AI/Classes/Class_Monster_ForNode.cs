using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike{
public class Monster {
	public GameObject inGameObject;
	public Vector3 prePosition;

	public Monster(GameObject _gameObject)
	{
		inGameObject = _gameObject;
	}

	public Vector3 GetPosition()
	{
		if(!inGameObject)
			return Vector3.zero;
		Vector3 _position = inGameObject.transform.position;
		return new Vector3(Mathf.Floor(_position.x)+0.5f, Mathf.Floor(_position.y)+0.5f, 0);
	}

	public Node MakeSelfToNode(Vector2 position, List<Node> nodes)
	{
		Node _selfNode = new Node(position);
		_selfNode.parentNode = _selfNode;
		nodes.Add(_selfNode);
		return _selfNode;
	}

	public Node MakeSelfToNode(List<Node> nodes)
	{
		Node _selfNode = new Node(GetPosition());
		_selfNode.parentNode = _selfNode;
		nodes.Add(_selfNode);
		return _selfNode;
	}
	

}
}