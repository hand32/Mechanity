using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
namespace roguelike
{
    public class DialogueSystem : MonoBehaviour
    {
        
        public Queue<string> sentences;

        public float typingSpeed = 0.02f;

        public GameObject panelObject;
        public Text nameText;
        public Text dialogueText;

        public bool isShowing;
        
        int nextIndex;

        void OnEnable()
        {
            typingSpeed = 0.03f;
            sentences = new Queue<string>();
            nameText.text = "";
            dialogueText.text = "";
            panelObject.SetActive(false);
        }

        public void StartDialogue(Dialogue dialogue)
        {
            if(isShowing)
                return;

            isShowing = true;
            
            panelObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            dialogueText.gameObject.SetActive(true);

            nameText.text = dialogue.name;
            sentences.Clear();
            foreach(string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
            DisplayNextSentence();
        }

        public void EndDialogue()
        {
            isShowing = false;
            panelObject.SetActive(false);
            nameText.gameObject.SetActive(false);
            dialogueText.gameObject.SetActive(false);
            this.StopCoroutine("Typing");
            dialogueText.text = "";
            nameText.text = "";
            StartCoroutine("NextDialogueTerm");
            //EndDialogueAnimation.
        }

        IEnumerator NextDialogueTerm()
        {
            float _elapsed = 0f;
            while(_elapsed < 0.3f)
            {
                yield return null;
                _elapsed += Time.deltaTime;
            }
            FindObjectOfType<DialogueTrigger>().isPlayerEnter = false;
        }

        public void DisplayNextSentence ()
        {
            if(sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            string sentence = sentences.Dequeue();
            StartCoroutine("Typing", sentence);
        }

        IEnumerator Typing(string sentence)
        {
            dialogueText.text = "";
            float _elapsed = 0f;
            foreach(char c in sentence)
            {
                while(true)
                {
                    yield return null;
                    //Sound 재생.
                    _elapsed += Time.deltaTime;
                    if(Input.anyKeyDown)
                    {
                        dialogueText.text = sentence;
                        break;
                    }
                    if(_elapsed >= typingSpeed)
                    {
                        dialogueText.text += c;
                        _elapsed = 0f;
                        break;
                    }
                }
                if(dialogueText.text == sentence)
                    break;
            }

            while(true)
            {
                yield return null;
                if(PlayerController.currentInput.interaction == KeyState.Down)
                {
                    DisplayNextSentence();
                    break;
                }
            }
        }


    }
}