using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace roguelike
{
    public class ItemContainer : MonoBehaviour
    {

        public List<Item> items;

        void Drop()
        {
			foreach(Item i in items)
			{
				if(Random.Range(0f, 100f) <= i.chance)
				{
					Instantiate(i.itemPrefab, transform.position, new Quaternion());
				}
			}
        }

		void Death()
		{
			Drop();
		}
    }

}