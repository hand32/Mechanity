using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike{
public class Spider : Monster {
	//스파이더는 자기가 이동해야하는 노드만을 받을 것.
	//그래서 이 클래스는 SpiderPathMaking 스크립트가 받은
	//GameObject의 정보를 저장하는 역할.
	//여기서 Spider 각 개체의 이동가능 노드를 SpiderPathMaking 스크립트로 넘겨주고
	//SpiderPathMaking 스크립트는 각 이동해야할 노드를 거미로 넘겨주자.
	public Monster_Spider_AI m_Script;

	
	public Spider(GameObject _gameObject) :base(_gameObject)
	{}

	public List<DirandRot> FindCollisionVectors()
	{
		if(!inGameObject.gameObject){
			return null;
		}

		var collisionState = inGameObject.GetComponent<Monster_Spider_AI>().collisionState;
		collisionState.Reset();
		inGameObject.GetComponent<Monster_Spider_AI>().CollisionCheck();
		List<DirandRot> _canMoveVectors = new List<DirandRot>();

		if(collisionState.down.HasCollision()){
			_canMoveVectors.Add(new DirandRot("Left", "Down"));
			_canMoveVectors.Add(new DirandRot("Right", "Down"));
		}
		if(collisionState.up.HasCollision())
		{
			_canMoveVectors.Add(new DirandRot("Left", "Up"));
			_canMoveVectors.Add(new DirandRot("Right", "Up"));
		}
		if(collisionState.right.HasCollision()){
			_canMoveVectors.Add(new DirandRot("Up", "Right"));
			_canMoveVectors.Add(new DirandRot("Down", "Right"));
		}
		if(collisionState.left.HasCollision()){
			_canMoveVectors.Add(new DirandRot("Up", "Left"));
			_canMoveVectors.Add(new DirandRot("Down", "Left"));
		}

		return _canMoveVectors;
		
	}
	public List<DirandRot> FindCanMoveVectors(SpiderNode selfNode)
	{
		List<DirandRot> _canMoveVectors = FindCollisionVectors();

		List<DirandRot> result = new List<DirandRot>();
		foreach(DirandRot dr in _canMoveVectors)
		{
			foreach(DirandRot selfDR in selfNode.canMoveVectors)
			{
				if(dr.rot.eulerAngles.y == selfDR.rot.eulerAngles.y)
				{
					if(Mathf.Round(Mathf.Abs(dr.rot.eulerAngles.z - selfDR.rot.eulerAngles.z)) != 180f)
					{
						if(!result.Contains(selfDR))
							result.Add(selfDR);
					}
				}
			}
		}
		

		return result;
	}

	
    
}
}