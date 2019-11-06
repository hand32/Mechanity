using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInteractionKey : MonoBehaviour
{

    public GameObject interactionKeyDisplayingPrefab;

    GameObject m_gameobject;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (!m_gameobject)
            {
				MakeObject();
            }
            m_gameobject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
			if(!m_gameobject)
			{
				MakeObject();
			}
            m_gameobject.SetActive(false);
        }
    }
    
    void Update()
    {
        if(transform.parent && transform.parent.transform.parent != null)
        {
            if(m_gameobject)
                m_gameobject.SetActive(false);
        }
    }

    void MakeObject()
    {
        m_gameobject = Instantiate(interactionKeyDisplayingPrefab);
        m_gameobject.transform.SetParent(gameObject.transform);
        m_gameobject.transform.localPosition = new Vector3(0f, 1f, 0f) + (Vector3)gameObject.GetComponentInParent<Collider2D>().offset;
        m_gameobject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
