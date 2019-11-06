using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace roguelike
{

    public class PlayerController : MonoBehaviour
    {
        public Inputlist inputlist;
        public static CurrentInput currentInput = new CurrentInput();

        public float getDamageDelay;

        Rigidbody2D m_rigidbody2D;
        SpriteRenderer m_spriteRenderer;
        bool gameStart = true;

        void OnEnable()
        {
            GetComponent<StatusManagement>().isDamageable = true;
            Class_SummonObject.MakeSummon();
            if (inputlist == null)
            {
                inputlist = new Inputlist();
                // /* auto input KeyCode reset.
                inputlist.right = KeyCode.RightArrow;
                inputlist.down = KeyCode.DownArrow;
                inputlist.left = KeyCode.LeftArrow;
                inputlist.up = KeyCode.UpArrow;
                inputlist.jump = KeyCode.S;
                inputlist.shoot = KeyCode.A;
                inputlist.fixMove = KeyCode.C;
                inputlist.reload = KeyCode.R;
                inputlist.dash = KeyCode.D;
                inputlist.interaction = KeyCode.F;
                // */
            }

            if (Class_Weapon.reloadFill == null)
            {
                Class_Weapon.reloadFill = GameObject.Find("Reload Fill");
                Class_Weapon.reloadFillImage = Class_Weapon.reloadFill.GetComponent<Image>();
                Class_Weapon.reloadFillImage.fillAmount = 0;
                Class_Weapon.reloadFill.transform.parent.gameObject.SetActive(false);
            }

            m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            m_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            currentInput.direction = DirectionEnum.Right;
            if (PlayerProgress.instance.isWeaponSaved)
            {
                if (PlayerProgress.instance.weaponObject)
                {
                    GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
                    GameObject pickedWeapon = null;
                    foreach (GameObject weapon in weapons)
                    {
                        if (weapon.GetComponent<Class_Weapon>().isPicked)
                        {
                            pickedWeapon = weapon;
                            break;
                        }
                    }
                    if (pickedWeapon != null)
                        Destroy(pickedWeapon);

                    GameObject savedWeapon = Instantiate(PlayerProgress.instance.weaponObject);
                    savedWeapon.transform.SetParent(transform);
                    savedWeapon.transform.localPosition = new Vector3(0f, 0f, 0f);
                    savedWeapon.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
                    savedWeapon.GetComponent<Rigidbody2D>().simulated = false;
                    savedWeapon.GetComponent<Class_Weapon>().isPicked = true;

                    Destroy(PlayerProgress.instance.weaponObject);
                }
            }
        }

        void Update()
        {
            //Input check.
            CurrentInputCheck(currentInput);

            // PlayerDirection check and Notify.
            DirectionEnum prePlayerDirection = currentInput.direction;
            PlayerDirectionCheck();
            //if(prePlayerDirection != currentInput.direction)

            toDoCheck();

        }

        public void GamePause()
        {
            gameStart = false;
        }
        public void GameStart()
        {
            gameStart = true;
        }


        void toDoCheck()
        {
            //Move Section
            if ((currentInput.right == KeyState.Held || currentInput.right == KeyState.Down) && currentInput.left == KeyState.None)
            {
                gameObject.SendMessage("Move", value:1f); //(Movefunction , horizonal).
            }
            else if ((currentInput.left == KeyState.Held || currentInput.left == KeyState.Down) && currentInput.right == KeyState.None)
            {
                gameObject.SendMessage("Move", value:-1f);
            }
            else
            {
                gameObject.SendMessage("Move", value:0f);
            }

            //Jump Section
            if (currentInput.jump == KeyState.Down)
            {
                gameObject.SendMessage("Jump", 0, SendMessageOptions.RequireReceiver);

            }
            else if (currentInput.jump == KeyState.Up)
            {
                gameObject.SendMessage("Jump", 2);
            }

            //Crouch Section
            /*
            if(currentInput.down == KeyState.Held || currentInput.down == KeyState.Down){
                gameObject.SendMessage("Crouch", currentInput.down);
            }
            else{
                gameObject.SendMessage("Crouch", currentInput.down);
            }
            */

            //Fire
            if (currentInput.shoot == KeyState.Down && !GetComponent<CharacterPhysics>().isDashing)
            {
                gameObject.BroadcastMessage("FireMessage", currentInput, SendMessageOptions.DontRequireReceiver);
            }
            else if (currentInput.shoot == KeyState.Held && !GetComponent<CharacterPhysics>().isDashing)
            {
                gameObject.BroadcastMessage("FireMessage", currentInput, SendMessageOptions.DontRequireReceiver);
            }

            //FixMove
            if (currentInput.fixMove == KeyState.Held)
            {
                gameObject.SendMessage("FixMove", currentInput.fixMove);
            }
            else if (currentInput.fixMove == KeyState.Up)
            {
                gameObject.SendMessage("FixMove", currentInput.fixMove);
            }

            //Reload
            if (currentInput.reload == KeyState.Held)
            {
                gameObject.BroadcastMessage("Reload");
            }

            //Dash
            if (currentInput.dash == KeyState.Down)
            {
                gameObject.SendMessage("Dash");
            }

        }

        //Check InputComponetns.cs
        void PlayerDirectionCheck()
        {
            // !! right -> down -> left -> up !!
            string combinedHeldState = currentInput.CombinedHeldStateForDirection();
            //Debug.Log("ConmbinedHeldState: " + combinedHeldState);
            if (combinedHeldState == "rightup")
            {
                currentInput.direction = DirectionEnum.RightUp;
            }
            else if (combinedHeldState == "right")
            {
                currentInput.direction = DirectionEnum.Right;
            }
            else if (combinedHeldState == "rightdown")
            {
                currentInput.direction = DirectionEnum.RightDown;
            }
            else if (combinedHeldState == "down")
            {
                currentInput.direction = DirectionEnum.Down;
            }
            else if (combinedHeldState == "downleft")
            {
                currentInput.direction = DirectionEnum.DownLeft;
            }
            else if (combinedHeldState == "left")
            {
                currentInput.direction = DirectionEnum.Left;
            }
            else if (combinedHeldState == "leftup")
            {
                currentInput.direction = DirectionEnum.LeftUp;
            }
            else if (combinedHeldState == "up")
            {
                currentInput.direction = DirectionEnum.Up;
            }
            else
            {
                currentInput.direction = Mathf.Round(transform.rotation.y) == 0f ? DirectionEnum.Right : DirectionEnum.Left;
            }
        }

        void CurrentInputCheck(CurrentInput _currentInput)
        {
            if(!gameStart)
                return;
            // Right
            if (Input.GetKeyDown(inputlist.right))
                _currentInput.right = KeyState.Down;
            else if (Input.GetKey(inputlist.right))
                _currentInput.right = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.right))
                _currentInput.right = KeyState.Up;
            else
                _currentInput.right = KeyState.None;

            // Down
            if (Input.GetKeyDown(inputlist.down))
                _currentInput.down = KeyState.Down;
            else if (Input.GetKey(inputlist.down))
                _currentInput.down = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.down))
                _currentInput.down = KeyState.Up;
            else
                _currentInput.down = KeyState.None;


            // Left
            if (Input.GetKeyDown(inputlist.left))
                _currentInput.left = KeyState.Down;
            else if (Input.GetKey(inputlist.left))
                _currentInput.left = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.left))
                _currentInput.left = KeyState.Up;
            else
                _currentInput.left = KeyState.None;


            // Up
            if (Input.GetKeyDown(inputlist.up))
                _currentInput.up = KeyState.Down;
            else if (Input.GetKey(inputlist.up))
                _currentInput.up = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.up))
                _currentInput.up = KeyState.Up;
            else
                _currentInput.up = KeyState.None;


            // Jump
            if (Input.GetKeyDown(inputlist.jump) || Input.GetKeyDown(KeyCode.Space))
                _currentInput.jump = KeyState.Down;
            else if (Input.GetKey(inputlist.jump) || Input.GetKey(KeyCode.Space))
                _currentInput.jump = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.jump) || Input.GetKeyUp(KeyCode.Space))
                _currentInput.jump = KeyState.Up;
            else
                _currentInput.jump = KeyState.None;


            // Shoot
            if (Input.GetKeyDown(inputlist.shoot))
                _currentInput.shoot = KeyState.Down;
            else if (Input.GetKey(inputlist.shoot))
                _currentInput.shoot = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.shoot))
                _currentInput.shoot = KeyState.Up;
            else
                _currentInput.shoot = KeyState.None;

            // FixMove
            if (Input.GetKeyDown(inputlist.fixMove))
                _currentInput.fixMove = KeyState.Down;
            else if (Input.GetKey(inputlist.fixMove))
                _currentInput.fixMove = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.fixMove))
                _currentInput.fixMove = KeyState.Up;
            else
                _currentInput.fixMove = KeyState.None;

            // Reload
            if (Input.GetKeyDown(inputlist.reload))
                _currentInput.reload = KeyState.Down;
            else if (Input.GetKey(inputlist.reload))
                _currentInput.reload = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.reload))
                _currentInput.reload = KeyState.Up;
            else
                _currentInput.reload = KeyState.None;

            // Dash
            if (Input.GetKeyDown(inputlist.dash) || Input.GetKeyDown(KeyCode.LeftShift))
                _currentInput.dash = KeyState.Down;
            else if (Input.GetKey(inputlist.dash) || Input.GetKey(KeyCode.LeftShift))
                _currentInput.dash = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.dash) || Input.GetKeyUp(KeyCode.LeftShift))
                _currentInput.dash = KeyState.Up;
            else
                _currentInput.dash = KeyState.None;

            //Interaction
            if (Input.GetKeyDown(inputlist.interaction))
                _currentInput.interaction = KeyState.Down;
            else if (Input.GetKey(inputlist.interaction))
                _currentInput.interaction = KeyState.Held;
            else if (Input.GetKeyUp(inputlist.interaction))
                _currentInput.interaction = KeyState.Up;
            else
                _currentInput.interaction = KeyState.None;

            //AnyKey
            _currentInput.anyKey = Input.anyKey;
            _currentInput.anyKeyDown = Input.anyKeyDown;
        }

        public void DelayDamaging(GameObject damagedObejct)
        {
            var statusManagement = damagedObejct.GetComponent<StatusManagement>();
            StartCoroutine("IgnoreDamaging", statusManagement);
            StartCoroutine("Blink");
            SendMessage("KnockBack", 1);
        }

        IEnumerator IgnoreDamaging(StatusManagement statusManagement)
        {
            //Debug.Log("damage ignore");
            statusManagement.isDamageable = false;
            yield return new WaitForSeconds(getDamageDelay);
            statusManagement.isDamageable = true;
            //Debug.Log("damage unignore");
        }

        IEnumerator Blink()
        {
            float preTime = Time.time;
            while (true)
            {
                m_spriteRenderer.color = Color.gray;
                yield return new WaitForSeconds(0.08f);
                m_spriteRenderer.color = new Color(180f / 255f, 180f / 255f, 180f / 255f);
                yield return new WaitForSeconds(0.08f);
                if (Time.time - preTime >= getDamageDelay)
                    break;
            }
            m_spriteRenderer.color = Color.white;
        }


    }
}