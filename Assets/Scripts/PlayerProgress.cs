using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerProgress : MonoBehaviour
{

    public static PlayerProgress instance
    {
        get
        {
            if (m_instance == null)
            {
                // 씬에 PlayerProgress 타입의 오브젝트가 이미 있는지 찾기
                m_instance = FindObjectOfType<PlayerProgress>();

                // 찾아도 없으면 하나 생성
                if (m_instance == null)
                {
                    m_instance = new GameObject("Player Progress").AddComponent<PlayerProgress>();
                }
            }
            return m_instance;
        }
    }

    private static PlayerProgress m_instance;

    public bool isStatSaved = false;


    public bool isDamageable;
    public float m_baseHealth;
    public float m_currentHealth;
    public float m_baseArmor;
    public float m_currentArmor;

    public bool isWeaponSaved = false;

    public GameObject weaponObject;


    void Awake()
    {
        // 자신이 싱글톤 오브젝트가 아니면 (이미 다른 싱글톤 오브젝트가 존재하면) 스스로를 파괴
        if (instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
	
	public void Reset()
	{
		instance.isStatSaved = false;
		instance.isWeaponSaved = false;
	}
}
