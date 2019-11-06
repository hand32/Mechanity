using UnityEngine;
using System.Collections.Generic;
namespace roguelike{
public enum KeyState {None = 0, Down =1, Held=2, Up=3};

/*
public class InputKey
{
    KeyState keyState {get; set;}
}
*/

public class CurrentInput
{
	public int keyCount = 6;
	public KeyState right {get; set;}
	public KeyState down {get; set;}
	public KeyState left {get; set;}
	public KeyState up {get; set;}
	public KeyState jump {get; set;}
	public KeyState shoot {get; set;}
    public KeyState fixMove {get; set;}
    public KeyState reload {get; set;}
    public KeyState dash {get; set;}
    public KeyState interaction {get; set;}
    public bool anyKey;
    public bool anyKeyDown;

    public DirectionEnum direction;
    
    public CurrentInput()
    {
        this.direction = DirectionEnum.Right;
    }

    public bool CheckMoveInput()
    {
        return  (CheckDownOrHeld(right) || CheckDownOrHeld(left) || CheckDownOrHeld(up) || CheckDownOrHeld(down));
    }

    public bool CheckDownOrHeld(KeyState _key)
    {
        return (_key == KeyState.Down) || (_key == KeyState.Held);
    }

    public Vector2 ToVector()
    {
        Vector2 ret = new Vector2(0, 0);
        string _tostring = direction.ToString();
        
        if(_tostring.Contains("Right"))
            ret += Vector2.right;
        if(_tostring.Contains("Left"))
            ret += Vector2.left;
        if(_tostring.Contains("Up"))
            ret += Vector2.up;
        if(_tostring.Contains("Down"))
            ret += Vector2.down;

        return ret;
    }

    public Quaternion ToQuat()
    {
        Vector2 _toVector = ToVector();
        Quaternion ret = new Quaternion();
        if(_toVector.x == 1 && _toVector.y == 0){
            ret = Quaternion.Euler(0, 0, 0);
        }
        else if(_toVector.x == 1 && _toVector.y == 1){
            ret = Quaternion.Euler(0, 0, 45);
        }
        else if(_toVector.x == 0 && _toVector.y == 1){
            ret = Quaternion.Euler(0, 0, 90);
        }
        else if(_toVector.x == -1 && _toVector.y == 1){
            ret = Quaternion.Euler(0, 0, 135);
        }
        else if(_toVector.x == -1 && _toVector.y == 0){
            ret = Quaternion.Euler(0, 0, 180);
        }
        else if(_toVector.x == -1 && _toVector.y == -1){
            ret = Quaternion.Euler(0, 0, 225);
        }
        else if(_toVector.x == 0 && _toVector.y == -1){
            ret = Quaternion.Euler(0, 0, 270);
        }
        else if(_toVector.x == 1 && _toVector.y == -1){
            ret = Quaternion.Euler(0, 0, 315);
        }
        return ret;
        //Debug.Log("Degrees : " +  Mathf.Atan(Mathf.Tan(_toVector.y / _toVector.x)));
        //return Quaternion.Euler(new Vector3(0, Mathf.Rad2Deg * Mathf.Atan(Mathf.Tan( _toVector.x == 0 ? Mathf.Infinity : _toVector.y / _toVector.x)), 0));
    }

    public string CombinedHeldStateForDirection()
    {
        string _result = "";
        if(right == KeyState.Held){
            _result += "right";
        }
        if(down == KeyState.Held){
            _result += "down";
        }
        if(left == KeyState.Held){
            _result += "left";
        }
        if(up == KeyState.Held){
            _result += "up";
        }
        return _result;
    }

    public List<KeyState> Listing() // Dictionary 자료형 쓰거나 모두 다 객체여야 함.
    {
        List<KeyState> _result = new List<KeyState>();
        _result.Add(right);
        _result.Add(down);
        _result.Add(left);
        _result.Add(up);
        _result.Add(jump);
        _result.Add(shoot);
        _result.Add(fixMove);
        _result.Add(reload);
        _result.Add(dash);
        _result.Add(interaction);
        return _result;
    }
    
}

public class Inputlist // To make Player modifiable input
{
	public KeyCode right {get; set;}
	public KeyCode down {get; set;}
	public KeyCode left {get; set;}
	public KeyCode up {get; set;}
	public KeyCode jump {get; set;}
	public KeyCode shoot {get; set;}
    public KeyCode fixMove {get; set;}
    public KeyCode reload {get; set;}
    public KeyCode dash {get; set;}
    public KeyCode interaction {get; set;}
}
}