using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace roguelike
{
    public class Summon_su004_FloatingShield : Class_SummonObject
    {
		public float revolutionCycleByMoveSpeed = 5;

        public override void Orbit()
        {
			float seta = timeFlow * 360f / (revolutionCycleByMoveSpeed / characterPhysics.moveSpeed);
            transform.position = characterPhysics.transform.position + new Vector3(1.5f * Mathf.Cos(Mathf.Deg2Rad * seta), 1.5f * Mathf.Sin(Mathf.Deg2Rad * seta), 0f);
            Vector3 _rot = new Vector3(0, 0, Vector2.Angle(Vector2.right, transform.position - characterPhysics.transform.position));
            if((transform.position - characterPhysics.transform.position).y < 0)
                _rot.z = 360f - _rot.z;
            transform.rotation = Quaternion.Euler(_rot);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            HitCheck hitCheck = other.GetComponent<HitCheck>();
            if(hitCheck != null)
            {
                if(hitCheck.shooter.tag != "Player")
                {
                    hitCheck.CrashMessage(transform);
                }
            }
        }
    }
}