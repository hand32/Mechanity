using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpriteController : MonoBehaviour
{

	SpriteRenderer m_SpriteRenderer;
	public List<Sprite> sprites;
	Queue<Sprite> spriteQueue = new Queue<Sprite>();

	void OnEnable()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		foreach(Sprite s in sprites)
		{
			spriteQueue.Enqueue(s);
		}
	}
	
	public void ChangeSprite()
	{
		if(spriteQueue.Count > 0)
			m_SpriteRenderer.sprite = spriteQueue.Dequeue();
	}
}
