using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRepeater : MonoBehaviour {

	public Transform TileGrid;
	public Vector2Int offset;

	bool make;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space) && !make)
		{
			make = true;
			Instantiate(gameObject, (Vector2)transform.position + offset, new Quaternion(), TileGrid);
		}
	}

}
