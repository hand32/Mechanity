using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace roguelike{

public class PathMaking : MonoBehaviour {

	public class TileCollision{
		List<List<bool>> array;

		public TileCollision(List<List<bool>> _array)
		{
			array = _array;
		}

		public List<List<bool>> GetArray()
		{
			return array;
		}
		
		public bool IsExist(params string[] _dirs)
		{
			foreach(string _dir in _dirs)
			{
				Vector2 vDir = new Vector2();
				if(_dir.Contains("Right"))
				{
					vDir += new Vector2(1, 0);
				}
				if(_dir.Contains("Down"))
				{
					vDir += new Vector2(0, -1);
				}
				if(_dir.Contains("Left"))
				{
					vDir += new Vector2(-1, 0);
				}
				if(_dir.Contains("Up"))
				{
					vDir += new Vector2(0, 1);
				}

				if(!array[1 + (int) vDir.x][1 + (int) vDir.y])
				{
					return false;
				}
			}
			return true;
		}
	
	}

	public class TilePosition{
		public bool exist;
		public Vector2 position;
		public static float offsetX;
		public static float offsetY;
		public TilePosition(bool _exist, Vector3 _position, float _offsetX, float _offsetY)
		{
			exist = _exist;
			position = _position;
			offsetX = _offsetX;
			offsetY = _offsetY;
		}
		public TilePosition()
		{
			exist = false;
		}

		public List<List<bool>> CheckCollision()
		{
			List<List<bool>> result = new List<List<bool>>();

			for(int i = -1; i <= 1; i++)
			{
				result.Add(new List<bool>());
				for(int j = -1; j<= 1; j++)
				{
					
					if((int)(position.x + offsetX) + i >= 0 && (int)(position.y + offsetY) + j >= 0 &&
					 	(int)(position.x + offsetX) + i <= platformTiles.Count && (int)(position.y + offsetY) + j <= platformTiles[0].Count
						  && (platformTiles[(int)(position.x + offsetX) + i][(int)(position.y + offsetY) + j].exist))
					{
						result[i+1].Add(true);
					}
					else{
						result[i+1].Add(false);
					}
				}
			}

			return result;
		}
		
	}

	public static List<List<TilePosition>> platformTiles = new List<List<TilePosition>>();
	public static bool getTilesFinish = false;
	public static List<Node> nodes = new List<Node>();
	public Transform playerPosition;
	public List<GameObject> monsters;
	public List<Monster> monstersClass;

	protected Tilemap tilemap;
	protected float offsetX;
	protected float offsetY;
	
    protected float playerColliderSizeX;

	/*
	void OnEnable()
	{
        monstersClass = new List<Monster>();
		playerColliderSizeX = playerPosition.gameObject.GetComponent<BoxCollider2D>().size.x;
        foreach(GameObject _gameObject in monsters)
        {
            monstersClass.Add(new Monster(_gameObject));
        }
		GetTiles();
		for(int i = 0; i < platformTiles.Count - 1; i++)
		{
			for(int j = 0; j < platformTiles[i].Count - 1; j++)
			{
	            MakeNodes(i, j);
            }
        }
		Debug.Log(nodes.Count +" Counts of SPider NOdes Made in Runtime.");
		foreach(Node a in nodes)
		{
			//Debug.Log(a.position + " Node have " + a.connectNodeList.Count + " connected Nodes.");
		}
	}
	*/

	public List<List<TilePosition>> GetTiles()
	{
		platformTiles = new List<List<TilePosition>>();
		tilemap = GetComponent<Tilemap>();
		BoundsInt bounds = tilemap.cellBounds;
		Vector3 toTileCenter = GetComponentInParent<Grid>().cellSize / 2;
		int i=0;
		int j=0;
		int cnt = 0;
		offsetX = -(bounds.xMin-1 + toTileCenter.x);
		offsetY = -(bounds.yMin-1 + toTileCenter.y);

		for(int x = bounds.xMin-1; x <= bounds.xMax+1; x++)
		{
			platformTiles.Add(new List<TilePosition>());
			j=0;
			for(int y = bounds.yMin-1; y <= bounds.yMax+1; y++)
			{
				Vector3Int localPosition = new Vector3Int(x, y, 0);
				Vector3 worldPosition = tilemap.CellToWorld(localPosition) + toTileCenter;
				if(tilemap.HasTile(localPosition))
				{
					platformTiles[i].Add(new TilePosition(true, worldPosition, offsetX, offsetY));
					//Debug.Log(platformTiles[i][j].position + " x: " + i + " y: " + j + " "+ worldPosition + " offset: " + offsetX + " " + offsetY); // 타일마다 포지션 Log 남김.
					cnt++;
				}
				else{		
					platformTiles[i].Add(new TilePosition(false, worldPosition, offsetX, offsetY));
				}
				j++;
			}
			i++;
		}
		Debug.Log("There are " + cnt + " Tiles");
		getTilesFinish = true;
		return platformTiles;
	}


	public void MakeNodes(int i, int j)
	{
		
	}

}
}