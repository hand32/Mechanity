using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace roguelike
{
    public class DirandRot
    {
        public Vector2 dir;
        public Quaternion rot;
        public DirandRot(Vector2 _dir, Quaternion _rot)
        {
            this.dir = _dir;
            this.rot = _rot;
        }

        public DirandRot(string _dir, string _rot)
        {
            Vector2 dirVector = new Vector2();
            Quaternion rotQuat = new Quaternion();
            if (_dir == "Right")
            {
                dirVector = Vector2.right;
                if (_rot == "Up")
                {
                    rotQuat = Quaternion.Euler(0f, 180f, 180f);
                }
                else if (_rot == "Down")
                {
                    rotQuat = Quaternion.Euler(0f, 0f, 0f);
                }
            }
            else if (_dir == "Down")
            {
                dirVector = Vector2.down;
                if (_rot == "Left")
                {
                    rotQuat = Quaternion.Euler(0f, 0f, 270f);
                }
                else if (_rot == "Right")
                {
                    rotQuat = Quaternion.Euler(0f, 180f, 270f);
                }
            }
            else if (_dir == "Left")
            {
                dirVector = Vector2.left;
                if (_rot == "Up")
                {
                    rotQuat = Quaternion.Euler(0f, 0f, 180f);
                }
                else if (_rot == "Down")
                {
                    rotQuat = Quaternion.Euler(0f, 180f, 0f);
                }
            }
            else if (_dir == "Up")
            {
                dirVector = Vector2.up;
                if (_rot == "Left")
                {
                    rotQuat = Quaternion.Euler(0f, 180f, 90f);
                }
                else if (_rot == "Right")
                {
                    rotQuat = Quaternion.Euler(0f, 0f, 90f);
                }
            }
            this.dir = dirVector;
            this.rot = rotQuat;

        }

        public bool AngleEquals(DirandRot obj1)
        {
            return (Mathf.Round(obj1.rot.eulerAngles.x) == Mathf.Round(this.rot.eulerAngles.x)
                                        && Mathf.Round(obj1.rot.eulerAngles.y) == Mathf.Round(this.rot.eulerAngles.y)
                                        && Mathf.Round(obj1.rot.eulerAngles.z) == Mathf.Round(this.rot.eulerAngles.z));
        }

        public static bool operator ==(DirandRot obj1, DirandRot obj2)
        {
            try
            {
                return (obj1.dir == obj2.dir && Mathf.Round(obj1.rot.eulerAngles.x) == Mathf.Round(obj2.rot.eulerAngles.x)
                                            && Mathf.Round(obj1.rot.eulerAngles.y) == Mathf.Round(obj2.rot.eulerAngles.y)
                                            && Mathf.Round(obj1.rot.eulerAngles.z) == Mathf.Round(obj2.rot.eulerAngles.z));
            }
            catch
            {
                return false;
            }
        }
        public static bool operator !=(DirandRot obj1, DirandRot obj2)
        {
            try
            {
                return (!(obj1.dir == obj2.dir) || !(Mathf.Round(obj1.rot.eulerAngles.x) == Mathf.Round(obj2.rot.eulerAngles.x)
                                            && Mathf.Round(obj1.rot.eulerAngles.y) == Mathf.Round(obj2.rot.eulerAngles.y)
                                            && Mathf.Round(obj1.rot.eulerAngles.z) == Mathf.Round(obj2.rot.eulerAngles.z)));
            }
            catch
            {
                return false;
            }

        }

    }

    public class SpiderPathMaking : PathMaking
    {
        public GameObject nodeSprite;
        public new static List<SpiderNode> nodes = new List<SpiderNode>();
        public new List<Spider> monstersClass;

        public SpiderNode MakeNode(int i, int j, List<SpiderNode> _nodes)
        {
            SpiderNode _node = null;
            if (!platformTiles[i][j].exist)
            {
                TileCollision collision = new TileCollision(platformTiles[i][j].CheckCollision());
                Vector2 tilePosition = platformTiles[i][j].position;
                List<DirandRot> _canMoveVectors = new List<DirandRot>();

                //Right Vector
                if (!collision.IsExist("Right"))
                {
                    if (collision.IsExist("Up") || collision.IsExist("RightUp"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.right, Quaternion.Euler(0f, 180f, 180f)));
                    }
                    if (collision.IsExist("Down") || collision.IsExist("RightDown"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.right, Quaternion.Euler(0f, 0f, 0f)));
                    }
                }
                //Down Vector
                if (!collision.IsExist("Down"))
                {
                    if (collision.IsExist("Left") || collision.IsExist("LeftDown"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.down, Quaternion.Euler(0f, 0f, 270f)));
                    }
                    if (collision.IsExist("Right") || collision.IsExist("RightDown"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.down, Quaternion.Euler(0f, 180f, 270f)));
                    }
                }
                //Left Vector
                if (!collision.IsExist("Left"))
                {
                    if (collision.IsExist("Up") || collision.IsExist("LeftUp"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.left, Quaternion.Euler(0f, 0f, 180f)));
                    }
                    if (collision.IsExist("Down") || collision.IsExist("LeftDown"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.left, Quaternion.Euler(0f, 180f, 0f)));
                    }
                }
                //Up Vector
                if (!collision.IsExist("Up"))
                {
                    if (collision.IsExist("Left") || collision.IsExist("LeftUp"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.up, Quaternion.Euler(0f, 180f, 90f)));
                    }
                    if (collision.IsExist("Right") || collision.IsExist("RightUp"))
                    {
                        _canMoveVectors.Add(new DirandRot(Vector2.up, Quaternion.Euler(0f, 0f, 90f)));
                    }
                }

                if (_canMoveVectors.Count != 0)
                {
                    _node = new SpiderNode(tilePosition, _canMoveVectors);

                    bool existPos = false;

                    if (_nodes != null)
                    {
                        foreach (SpiderNode a in _nodes)
                        {
                            if (a.position == tilePosition)
                            {
                                _node = a;
                                existPos = true;
                                break;
                            }
                        }
                    }

                    if (_nodes != null && !_nodes.Contains(_node) && _node != null && !existPos)
                    {
                        if (nodeSprite)
                            Instantiate(nodeSprite, _node.position, new Quaternion());
                        _nodes.Add(_node);
                    }
                }
            }


            else
            {
                /*
                    Debug.Log(platformTiles[i][j].position + "'s collisionState. " +i +", "+j);
                    foreach(List<bool> a in platformTiles[i][j].CheckCollision())
                    {
                        string s = "";
                        for(int z = 0; z<3;  z++)
                        {
                            if(a[z])
                                s+= "T ";
                            else   
                                s+= "F ";
                        }
                        Debug.Log(s);
                    }
                */
            }
            return _node;

            if (false) // 채워진 tile 기준으로 노드 만든 이전 코드.
            {
                /*
                if(platformTiles[i][j].exist)
                {
                    TileCollision collision = new TileCollision(platformTiles[i][j].CheckCollision());
                    Vector2 tilePosition = platformTiles[i][j].position;

                    //Make InTurn Nodes.
                    //Right, Up exist. RightUp node.
                    if(collision.IsExist("Right", "Up") && !collision.IsExist("RightUp")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.right);
                        _canMoveVectors.Add(Vector2.up);
                        nodes.Add(new SpiderNode(tilePosition + new Vector2(1, 1), _canMoveVectors, 0));
                    }
                    //Left, Up exist. LeftUP node.
                    if(collision.IsExist("Left", "Up") && !collision.IsExist("LeftUp")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.up);
                        _canMoveVectors.Add(Vector2.left);
                        nodes.Add(new SpiderNode(tilePosition + new Vector2(-1, 1), _canMoveVectors, 0));
                    }
                    //Left, Down exist. LeftDown node.
                    if(collision.IsExist("Left", "Down") && !collision.IsExist("LeftDown")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.left);
                        _canMoveVectors.Add(Vector2.down);
                        nodes.Add(new SpiderNode(tilePosition + new Vector2(-1, -1), _canMoveVectors, 0));
                    }
                    //Right, Down exist. RightDown node.
                    if(collision.IsExist("Right", "Down") && !collision.IsExist("RightDown")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.right);
                        _canMoveVectors.Add(Vector2.down);
                        nodes.Add(new SpiderNode(tilePosition + new Vector2(1, -1), _canMoveVectors, 0));
                    }


                    //Make OutTurn Nodes.
                    //Right, Up empty. RightUp node.
                    if(!collision.IsExist("Right") && !collision.IsExist("Up") && !collision.IsExist("Right")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.left);
                        _canMoveVectors.Add(Vector2.down);
                        //nodes.Add(new SpiderNode(tilePosition + new Vector2(1, 1), _canMoveVectors, 1));
                    }
                    //Left, Up empty. LeftUp node.
                    if(!collision.IsExist("Left") && !collision.IsExist("Up") && !collision.IsExist("LeftUP")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.right);
                        _canMoveVectors.Add(Vector2.down);
                        //nodes.Add(new SpiderNode(tilePosition + new Vector2(-1, 1), _canMoveVectors, 1));
                    }
                    //Left, Down empty. LeftDown node.
                    if(!collision.IsExist("Left") && !collision.IsExist("Down") && !collision.IsExist("LeftDown")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.right);
                        _canMoveVectors.Add(Vector2.up);
                        //nodes.Add(new SpiderNode(tilePosition + new Vector2(-1, -1), _canMoveVectors, 1));
                    }
                    //Right, Down empty. RightDown node.
                    if(!collision.IsExist("Right") && !collision.IsExist("Down") && !collision.IsExist("RightDown")){
                        List<Vector2> _canMoveVectors = new List<Vector2>();
                        _canMoveVectors.Add(Vector2.left);
                        _canMoveVectors.Add(Vector2.up);
                        //nodes.Add(new SpiderNode(tilePosition + new Vector2(1, -1), _canMoveVectors, 1));
                    }
                    //Visualize CollisionState of Tiles
                    /*
                    Debug.Log(platformTiles[i][j].position + "'s collisionState.");
                    foreach(List<bool> a in collision.GetArray())
                    {
                        string s = "";
                        for(int z = 0; z<3;  z++)
                        {
                            if(a[z])
                                s+= "T ";
                            else   
                                s+= "F ";
                        }
                        Debug.Log(s);
                    }
                    //(별/)
                }
                */
            }
        }


        void Start()
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform;

            //Spider 자동 리스팅.
            monstersClass = new List<Spider>();
            int cnt = 0;
            foreach (GameObject _gameObject in GameObject.FindObjectsOfType<GameObject>())
            {
                if (_gameObject.GetComponent<Monster_Spider_AI>())
                {
                    monsters.Add(_gameObject);
                    monstersClass.Add(new Spider(_gameObject));
                    monstersClass[cnt].m_Script = _gameObject.GetComponent<Monster_Spider_AI>();
                    cnt++;
                }
            }

            //Tile 위치들 리스팅.
            playerColliderSizeX = playerPosition.gameObject.GetComponent<CapsuleCollider2D>().size.x;
            GetTiles();

            //Make initial SpiderNodes.
            for (int i = 1; i < platformTiles.Count - 1; i++)
            {
                for (int j = 1; j < platformTiles[i].Count - 1; j++)
                {
                    MakeNode(i, j, nodes);
                }
            }

            Debug.Log(nodes.Count + " Counts of SPider NOdes Made in Runtime.");
            StartCoroutine("FindNode");

        }


        public DirandRot GetNextNode(Spider _monster)
        {
            int loopcount = 0;
            List<SpiderNode> _nodes = new List<SpiderNode>(nodes);

            List<SpiderNode> nodeOpenList = new List<SpiderNode>();
            List<SpiderNode> nodeClosedList = new List<SpiderNode>();


            // 플레이어가 공중에 있을때 원래 있던 땅 자리를 목적지로 만드려는 것.
            Vector2 playerNodePosition = new Vector2();
            int playerXindex = Mathf.FloorToInt(playerPosition.position.x + 0.5f + offsetX);
            int playerYindex = Mathf.FloorToInt(playerPosition.position.y + 0.5f + offsetY);
            for (int i = 0; i < (platformTiles[playerXindex].Count - playerYindex); i++)
            {
                if (platformTiles[playerXindex][playerYindex - i - 1].exist)
                {
                    playerNodePosition = new Vector2(playerXindex - offsetX, playerYindex - i - offsetY);
                    break;
                }

                else if (Mathf.Abs(playerXindex - offsetX - playerPosition.position.x) + playerColliderSizeX / 2 > 0.5f)
                {
                    int _playerXindex = playerXindex + (playerXindex - offsetX - playerPosition.position.x > 0 ? -1 : 1);
                    if (platformTiles[_playerXindex][playerYindex - i - 1].exist && !platformTiles[_playerXindex][playerYindex - i].exist)
                    {
                        playerNodePosition = new Vector2(_playerXindex - offsetX, playerYindex - i - offsetY);
                        break;
                    }
                }
            }
            SpiderNode goalNode =
                MakeNode((int)(playerNodePosition.x + offsetX), (int)(playerNodePosition.y + offsetY)
                            , _nodes);

            //현재 길 찾고있는 Spider의 속한 그리드의 중심에 노드를 만들기 위함. 원래 그자리에 노드가 있으면 합침.
            SpiderNode selfNode =
                MakeNode((int)(_monster.GetPosition().x + offsetX), (int)(_monster.GetPosition().y + offsetY)
                            , _nodes);
            if (selfNode == null || goalNode == null) //spider 주변에 벽이 없거나, 플레이어 아래 바닥이 끝까지 없을 때.
            {
                _monster.inGameObject.SendMessage("Fall");
                return new DirandRot(new Vector2(0, 0), Quaternion.Euler(0,0,0));
            }
            selfNode.parentNode = selfNode;
            selfNode.SetGH(goalNode);
            nodeOpenList.Add(selfNode);

            SpiderNode nearestNode = null;
            while (loopcount < 100)
            {
                loopcount++;
                float minF = Mathf.Infinity;
                SpiderNode minFNode = null;
                foreach (SpiderNode _node in nodeOpenList)
                {
                    float _currentF = _node.GetF();
                    if (minFNode == null || _currentF < minF && _node.H <= _monster.m_Script.spiderInfo.findRadius)
                    {
                        minF = _currentF;
                        minFNode = _node;
                    }
                    else if (_currentF == minF)
                    {
                        if (Random.Range(0, 2) == 1)
                        {
                            minF = _currentF;
                            minFNode = _node;
                        }
                    }
                }
                //Debug.Log(minFNode.position +" is 탐색됨. " + minFNode.FindNextNode(_nodes).Count);
                if (!nodeClosedList.Contains(minFNode))
                    nodeClosedList.Add(minFNode);
                nodeOpenList.Remove(minFNode);
                if (nearestNode == null || nearestNode.H > minFNode.H)
                {
                    nearestNode = minFNode;
                }
                foreach (SpiderNode _node in minFNode.FindNextNode(_nodes, _monster))
                {
                    if (!nodeClosedList.Contains(_node))
                    {
                        _node.parentNode = minFNode;
                        if (_node == goalNode)
                        {
                            if (selfNode.position == goalNode.position)
                                return new DirandRot(Vector2.zero, new Quaternion());
                            SpiderNode getBackNode = _node;
                            loopcount = 0;
                            while (loopcount < 100)
                            {
                                loopcount++;
                                if (getBackNode.parentNode == selfNode)
                                {
                                    return getBackNode.parentDR;
                                }
                                getBackNode = getBackNode.parentNode;
                            }
                            Debug.Log("Loop count over to Goal Node. " + goalNode.position);
                            return new DirandRot(Vector2.zero, new Quaternion());
                        }
                        _node.SetGH(goalNode);
                        if (!nodeOpenList.Contains(_node))
                        {
                            nodeOpenList.Add(_node);
                        }
                    }
                }

                if (nodeOpenList.Count == 0)
                {
                    Debug.Log(_monster.inGameObject.name + " can't find the path.");
                    if (nearestNode.position == selfNode.position)
                        return new DirandRot(Vector2.zero, new Quaternion());
                    SpiderNode getBackNode = nearestNode;
                    loopcount = 0;
                    while (loopcount < 100)
                    {
                        loopcount++;
                        if (getBackNode.parentNode == selfNode)
                        {
                            return getBackNode.parentDR;
                        }
                        getBackNode = getBackNode.parentNode;
                    }
                    Debug.Log("Loop count over to Nearest Node.");
                    return new DirandRot(Vector2.zero, new Quaternion());
                }
            }
            //Debug.Log("Loop count over in finding path. " + goalNode.position);
            if (nearestNode.position == selfNode.position)
                return new DirandRot(Vector2.zero, new Quaternion());
            SpiderNode _getBackNode = nearestNode;
            loopcount = 0;
            while (loopcount < 100)
            {
                loopcount++;
                if (_getBackNode.parentNode == selfNode)
                {
                    return _getBackNode.parentDR;
                }
                _getBackNode = _getBackNode.parentNode;
            }
            Debug.Log("Can't Find Anything with " + _monster.inGameObject.name);
            return new DirandRot(Vector2.zero, new Quaternion());
        }


        IEnumerator FindNode()
        {
            while (true)
            {
                foreach (Spider _monster in monstersClass)
                {
                    if (_monster.inGameObject)
                    {
                        if (_monster.m_Script.findPlayer && !_monster.m_Script.isCornering && !_monster.m_Script.isShooting)
                        {
                            if (_monster.prePosition != _monster.GetPosition())
                            {
                                _monster.prePosition = _monster.GetPosition();
                            }
                            DirandRot _nextDR = GetNextNode(_monster);
                            if (_nextDR.Equals(null))
                                _nextDR = new DirandRot(new Vector2(), Quaternion.Euler(0, 0, 0));
                            _monster.inGameObject.GetComponent<Monster_Spider_AI>().PassNextNode(_nextDR);
                        }
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

    }
}