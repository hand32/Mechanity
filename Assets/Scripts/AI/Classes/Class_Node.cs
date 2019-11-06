using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace roguelike{

public class Node
{
	public Vector2 position;
	public float G;
	public float H;
	public Node parentNode;
	public List<Node> connectNodeList = new List<Node>();
	public List<Vector2> canMoveVectors;

	public Node(Vector2 _position)
	{
		G = H = 0;
		this.position = _position;
	}

	public void SetGH(Node goal)
	{
		this.G = parentNode.G + (parentNode.position - this.position).magnitude;
		this.H = (goal.position - this.position).magnitude;
	}

	public float GetF()
	{
		//SetGH(goal);
		return G + H;
	}

	public Vector3 positionToVector3()
	{
		return new Vector3(position.x, position.y, 0);
	}

	public List<Node> FindNextNode(List<Node> nodes)
	{
		Debug.Log("Im NOde FindNOde");
		return new List<Node>();
	}
}

}
// virtual 이용해서 FindNextNode 를 모든 종류의 AI에 사용할 수 있게 하자.
// 노드만 확실히 만들어 주면 F = G + H 식으로 가장 최단거리를 찾을 수 있을 것이라 기대.