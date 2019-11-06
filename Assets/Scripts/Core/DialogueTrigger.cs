using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{
    public class DialogueTrigger : MonoBehaviour
    {
        public List<Dialogue> dialogueList;
        public Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
        public bool isPlayerEnter;
        Dialogue lastDialogue;

        GameObject playerObject;

        void OnEnable()
        {
            foreach(Dialogue _d in dialogueList)
            {
                dialogueQueue.Enqueue(_d);
            }
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                playerObject = other.gameObject;
                isPlayerEnter = true;
                Debug.Log("Press F");
                StartCoroutine("PlayerEnter");
                StartCoroutine("PlayerStayCheck");
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                isPlayerEnter = false;
                PlayerExit();
            }
        }
        IEnumerator PlayerStayCheck()
        {
            while(true)
            {
                yield return null;
                if(!isPlayerEnter)
                {
                    isPlayerEnter = true;
                    StartCoroutine("PlayerEnter");
                }
            }
        }

        IEnumerator PlayerEnter()
        {
            while(isPlayerEnter)
            {
                if(PlayerController.currentInput.interaction == KeyState.Down)
                {
                    playerObject.GetComponent<StatusManagement>().m_currentArmor = playerObject.GetComponent<StatusManagement>().m_baseArmor;
                    if(dialogueQueue.ToArray().Length == 1)
                        lastDialogue = dialogueQueue.Dequeue();

                    if(dialogueQueue.ToArray().Length  == 0)
                    {
                        TriggerDialogue(lastDialogue);
                        break;
                    }
                    else
                    {
                        TriggerDialogue(dialogueQueue.Dequeue());
                        break;
                    }
                }
                yield return null;
            }
        }

        void PlayerExit()
        {
            //다 없애기.
            Debug.Log("Player Exit.");
            StopCoroutine("PlayerEnter");
            StopCoroutine("PlayerStayCheck");
            EndDialouge();
        }

        public void TriggerDialogue(Dialogue dialogue)
        {
            FindObjectOfType<DialogueSystem>().StartDialogue(dialogue);
        }

        public void EndDialouge()
        {
            FindObjectOfType<DialogueSystem>().EndDialogue();
        }
    }
}