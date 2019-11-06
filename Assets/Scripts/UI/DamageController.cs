using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageController : MonoBehaviour {

	public Text damageText;
	public float exitTime = 0.8f;

	void Awake()
	{
		damageText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
		
	}
	void Start()
	{
		StartCoroutine("ExitDamage");
	}

	public void DamageUpdate(float damage)
	{
		damageText.text = damage.ToString();
	}

	IEnumerator ExitDamage()
	{
		transform.position += new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0f);
		Destroy(gameObject, exitTime);
		float elapsed = 0f;
		Color _color = damageText.color;
		while(true)
		{
			yield return null;
			elapsed += Time.deltaTime;
			if(elapsed > exitTime)
				elapsed = exitTime;
			damageText.color = new Color(_color.r, _color.g, _color.b, 1-(1/exitTime * elapsed));
			transform.position += new Vector3(0, 0.4f * Time.deltaTime, 0);
		}
	}
}
