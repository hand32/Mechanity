using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class Class_SummonObject : MonoBehaviour
    {
        public static List<GameObject> summonList;

        private SpriteRenderer m_spriteRenderer;

        protected SpriteRenderer spriteRenderer
        {
            get
            {
                if(m_spriteRenderer == null)
                    m_spriteRenderer = GetComponent<SpriteRenderer>();
                return m_spriteRenderer;
            }
        }
        
        private CharacterPhysics m_characterPhysics;
        protected CharacterPhysics characterPhysics
        {
            get
            {
                if(m_characterPhysics == null)
                    m_characterPhysics = GameObject.FindWithTag("Player").GetComponent<CharacterPhysics>();
                return m_characterPhysics;
            }
        }

        public static float mainSeta = 0f;
        public float timeFlow = 0f;

        public static void MakeSummon()
        {
            if (summonList == null)
            {
                summonList = new List<GameObject>();
                for(int i=0; i<6; i++)
                {
                    summonList.Add(new GameObject());
                }
            }
        }

        void Update()
        {
            Orbit();
            mainSeta += 50 * Time.deltaTime;
            timeFlow += Time.deltaTime;
        }

        public virtual void Orbit()
        {
            float seta = mainSeta + summonList.IndexOf(gameObject) * 72f;
            transform.position = characterPhysics.transform.position + new Vector3(1.5f * Mathf.Cos(Mathf.Deg2Rad * seta),
                                                                            1.5f * Mathf.Sin(Mathf.Deg2Rad * seta), 0f);
        }

    }
}