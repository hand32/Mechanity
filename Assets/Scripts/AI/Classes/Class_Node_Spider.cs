using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace roguelike{
public class SpiderNode : Node
{
	public new SpiderNode parentNode;
	public DirandRot parentDR;
	public new List<SpiderNode> connectNodeList = new List<SpiderNode>();
	public new List<DirandRot> canMoveVectors = new List<DirandRot>();



	public SpiderNode(Vector2 _position, List<DirandRot> _canMoveVectors) : base(_position)
	{
		this.canMoveVectors = _canMoveVectors;
	}
	public SpiderNode(Vector2 _position) : base(_position)
	{}

	public void SetGH(SpiderNode goal)
	{
		this.G = parentNode.G + (parentNode.position - this.position).magnitude;
		this.H = (goal.position - this.position).magnitude;
	}
	public void MixDir(SpiderNode _node)
	{
		foreach(DirandRot _dir in _node.canMoveVectors)
		{
			if(_dir == null)
				continue;
			if(!this.canMoveVectors.Contains(_dir)){
				this.canMoveVectors.Add(_dir);
			}
		}
	}

	public List<SpiderNode> FindNextNode(List<SpiderNode> nodes, Spider _monster)
	{
		List<SpiderNode> _connectNodeList = new List<SpiderNode>();

		if(parentNode == this)
		{
			List<DirandRot> _canMoveVectors = _monster.FindCanMoveVectors(this);
			foreach(DirandRot _DR in _canMoveVectors)
			{
				if(_DR == null)
					continue;
				foreach(DirandRot _parentDR in _monster.FindCollisionVectors())
				{
					if(_parentDR == null)
						continue;
					parentDR = _parentDR;
					Vector2 nextNodePosition = this.position;
					DirandRot downDR;
					if(parentDR.rot.eulerAngles.y == 0f)
					{
						downDR = new DirandRot(new Vector2(parentDR.dir.y, -parentDR.dir.x),
											Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z - 90f));
					}
					else
					{
						downDR = new DirandRot(new Vector2(-parentDR.dir.y, parentDR.dir.x),
											Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z - 90f));
					}
					
					DirandRot checkDR;
					checkDR = new DirandRot(new Vector2(-parentDR.dir.y, parentDR.dir.x),
						parentDR.rot.eulerAngles.y == 0f ? Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z+90f)
															: Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z-90f));
					
					
					if(_DR == parentDR) // 직진할 경우
					{
						
						bool isFloorExist = true;
						foreach(DirandRot d in _canMoveVectors)
						{
							if(d == null)
								continue;
							if(d == downDR) // 바닥이 없을 경우.
							{
								isFloorExist = false;
								break;
							}
						}
						if(isFloorExist){
							nextNodePosition = this.position + _DR.dir;
							checkDR = parentDR;
						}
					}
					if(this.position == nextNodePosition && _DR == checkDR) //반시계 90도 회전.
					{
						bool canTurn = true;
						if(parentDR.rot.eulerAngles.y != 0f)
						{
							
							foreach(DirandRot d in _canMoveVectors)
							{
								if(d == null)
									continue;
								if(d == downDR) // 바닥이 없을 경우.
								{
									canTurn = false;
									break;
								}
								else if(d == parentDR) // 앞쪽 벽이 없을 경우.
								{
									canTurn = false;
									break;
								}
							}
						}
						if(canTurn){
							nextNodePosition = this.position + checkDR.dir;
						}
					}
					else if(this.position == nextNodePosition)
					{ // 시계 90도 회전. 아마도.
						checkDR = new DirandRot(new Vector2(parentDR.dir.y, -parentDR.dir.x),
							parentDR.rot.eulerAngles.y == 0f ? Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z-90f)
																: Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z+90f));
						if(_DR == checkDR) //가는 경로 바닥에 타일 있는지 확인 조건 추가.
						{
							bool canTurn = true;
							if(parentDR.rot.eulerAngles.y == 0f)
							{
								foreach(DirandRot d in _canMoveVectors)
								{
									if(d == null)
										continue;
									if(d == downDR) // 바닥이 없을 경우.
									{
										canTurn = false;
										break;
									}
									else if(d == parentDR) // 앞쪽 벽이 없을 경우.
									{
										canTurn = false;
										break;
									}
								}
							}
							if(canTurn){
								nextNodePosition = this.position + checkDR.dir;
							}
						}
					}

					if(nextNodePosition != this.position)
					{
						foreach(SpiderNode _node in nodes)
						{
							if(_node.position == nextNodePosition)
							{
								_node.parentDR = new DirandRot(checkDR.dir, checkDR.rot);
								_connectNodeList.Add(_node);
								break;
							}
						}
					}
				}
			}
		}
		else
		{
			foreach(DirandRot _DR in canMoveVectors)
			{
				if(_DR == null)
					continue;
				Vector2 nextNodePosition = this.position;
				DirandRot downDR;
				if(parentDR.rot.eulerAngles.y == 0f)
				{
					downDR = new DirandRot(new Vector2(parentDR.dir.y, -parentDR.dir.x),
										Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z - 90f));
				}
				else
				{
					downDR = new DirandRot(new Vector2(-parentDR.dir.y, parentDR.dir.x),
										Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z - 90f));
				}
				
				DirandRot checkDR;
				checkDR = new DirandRot(new Vector2(-parentDR.dir.y, parentDR.dir.x),
					parentDR.rot.eulerAngles.y == 0f ? Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z+90f)
														: Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z-90f));
				
				
				if(_DR == parentDR) // 직진할 경우
				{
					
					bool isFloorExist = true;
					foreach(DirandRot d in canMoveVectors)
					{
						if(d == null)
							continue;
						if(d == downDR) // 바닥이 없을 경우.
						{
							isFloorExist = false;
							break;
						}
					}
					if(isFloorExist){
						nextNodePosition = this.position + _DR.dir;
						checkDR = parentDR;
					}
				}
				if(this.position == nextNodePosition && _DR == checkDR) //반시계 90도 회전.
				{
					bool canTurn = true;
					if(parentDR.rot.eulerAngles.y != 0f)
					{
						
						foreach(DirandRot d in canMoveVectors)
						{
							if(d == null)
								continue;
							if(d == downDR) // 바닥이 없을 경우.
							{
								canTurn = false;
								break;
							}
							else if(d == parentDR) // 앞쪽 벽이 없을 경우.
							{
								canTurn = false;
								break;
							}
						}
					}
					if(canTurn){
						nextNodePosition = this.position + checkDR.dir;
					}
				}
				else if(this.position == nextNodePosition)
				{ // 시계 90도 회전. 아마도.
					checkDR = new DirandRot(new Vector2(parentDR.dir.y, -parentDR.dir.x),
						parentDR.rot.eulerAngles.y == 0f ? Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z-90f)
															: Quaternion.Euler(parentDR.rot.eulerAngles.x, parentDR.rot.eulerAngles.y, parentDR.rot.eulerAngles.z+90f));
					if(_DR == checkDR) //가는 경로 바닥에 타일 있는지 확인 조건 추가.
					{
						bool canTurn = true;
						if(parentDR.rot.eulerAngles.y == 0f)
						{
							foreach(DirandRot d in canMoveVectors)
							{
								if(d == null)
									continue;
								if(d == downDR) // 바닥이 없을 경우.
								{
									canTurn = false;
									break;
								}
								else if(d == parentDR) // 앞쪽 벽이 없을 경우.
								{
									canTurn = false;
									break;
								}
							}
						}
						if(canTurn){
							nextNodePosition = this.position + checkDR.dir;
						}
					}
				}

				if(nextNodePosition != this.position)
				{
					foreach(SpiderNode _node in nodes)
					{
						if(_node.position == nextNodePosition)
						{
							_node.parentDR = new DirandRot(checkDR.dir, checkDR.rot);
							_connectNodeList.Add(_node);
							break;
						}
					}
				}
			}
		}
		
		if(_connectNodeList.Count == 0)
		{
			Debug.Log(this.position + " : this SpiderNode can't find connected Node!");
		}
		
		return _connectNodeList;
	}
		
}

}