using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace roguelike
{
    [System.Serializable]
    public class BackGroundsAndBBoxsClass{
        public GameObject BackGround;
        public GameObject LeftDownPosition;
        public GameObject RightUpPosition;
        public bool moveBasedSpeed = true;
        public Vector2 speed;
    }

    public class BackGroundController : MonoBehaviour
    {
        public float orthographicSize;
        public List<BackGroundsAndBBoxsClass> BackGroundsAndBBoxs;
        Dictionary<GameObject, Vector2> ratio = new Dictionary<GameObject, Vector2>();
        Cinemachine.CinemachineVirtualCamera VCam;

        public new GameObject camera;
        Vector3 preCameraPosition;
        Vector2 mapSize;
        List<float> mapBounds;

        bool initialize = false;
        bool initialize2 = false;

        GameObject playerObject;

        private bool startMove;

        void OnEnable()
        {
            playerObject = GameObject.FindWithTag("Player");
            camera = GameObject.FindGameObjectWithTag("MainCamera");
            VCam = camera.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
            Invoke("StartMove", 0.5f);
        }

        void StartMove()
        {
            startMove = true;
        }

        void OnGUI()
        {
            float _len = 9999;
            if (!startMove)
                return;
            if(!initialize)
            {
                Vector3 cp = camera.transform.position;
                Vector3 pp = playerObject.transform.position;
                cp.z = pp.z = 0;
                _len = (cp - pp).magnitude;
            }
            //PathMaking.GetTiles() 가 Start() 시점에 실행돼서 여기로 옮김.
            if (!initialize && PathMaking.getTilesFinish)
            {
                initialize = true;
                preCameraPosition = camera.transform.position;
                preCameraPosition.z = 0;

                float minX = Mathf.Infinity;
                float minY = Mathf.Infinity;
                float maxX = -Mathf.Infinity;
                float maxY = -Mathf.Infinity;
                for (int i = 0; i < PathMaking.platformTiles.Count; i++)
                {
                    for (int j = 0; j < PathMaking.platformTiles[i].Count; j++)
                    {
                        if (!PathMaking.platformTiles[i][j].exist)
                        {
                            if (i - 1 >= 0 &&
                                PathMaking.platformTiles[i - 1][j].exist)
                            {
                                if (PathMaking.platformTiles[i][j].position.x < minX)
                                    minX = PathMaking.platformTiles[i][j].position.x;
                            }

                            if (i + 1 < PathMaking.platformTiles.Count &&
                                PathMaking.platformTiles[i + 1][j].exist)
                            {
                                if (PathMaking.platformTiles[i][j].position.x > maxX)
                                    maxX = PathMaking.platformTiles[i][j].position.x;
                            }

                            if (j - 1 >= 0 &&
                                PathMaking.platformTiles[i][j - 1].exist)
                            {
                                if (PathMaking.platformTiles[i][j].position.y < minY)
                                    minY = PathMaking.platformTiles[i][j].position.y;
                            }

                            if (j + 1 < PathMaking.platformTiles[i].Count &&
                                PathMaking.platformTiles[i][j + 1].exist)
                            {
                                if (PathMaking.platformTiles[i][j].position.y > maxY)
                                    maxY = PathMaking.platformTiles[i][j].position.y;
                            }
                        }
                    }
                }
                maxX += 0.5f;
                minX -= 0.5f;
                maxY += 0.5f;
                minY -= 0.5f;
                //float orthographicSize = camera.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize;
                if(orthographicSize == 0)
                    orthographicSize = 10;
                mapSize = new Vector2(maxX - minX, maxY - minY);
                Debug.Log("MapSize : " + mapSize);
                mapBounds = new List<float>();
                mapBounds.Add(minX);
                mapBounds.Add(minY);
                mapBounds.Add(maxX);
                mapBounds.Add(maxY);
                
                
                foreach (BackGroundsAndBBoxsClass _g in BackGroundsAndBBoxs)
                {
                    if(_g.BackGround == null || _g.LeftDownPosition == null || _g.RightUpPosition == null || _g.moveBasedSpeed)
                        continue;
                    GameObject g = _g.BackGround;
                    float width = _g.RightUpPosition.transform.position.x - _g.LeftDownPosition.transform.position.x;
                    float height = _g.RightUpPosition.transform.position.y - _g.LeftDownPosition.transform.position.y;
                    Debug.Log(width +", " + height);
                    ratio.Add(g, new Vector2((width - orthographicSize *2) /mapSize.x, (height - orthographicSize *2 *9f/16f) /mapSize.y));
                }         
            }
            if (initialize && !initialize2)
            {
                initialize2 = true;                   
                Vector2 _distance = new Vector2((mapBounds[0] + mapBounds[2]) / 2 - camera.transform.position.x ,
                                                       (mapBounds[1] + mapBounds[3]) / 2 - camera.transform.position.y);
                foreach (BackGroundsAndBBoxsClass _g in BackGroundsAndBBoxs)
                {
                    if (_g == null || _g.moveBasedSpeed)
                        continue;
                    GameObject g = _g.BackGround;
                    Vector3 center = new Vector2();
                    /*
                    SpriteRenderer[] sprites = g.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer s in sprites)
                    {
                        center += s.bounds.center;
                    }
                    center /= sprites.Length;
                    */
                    center = (_g.LeftDownPosition.transform.position + _g.RightUpPosition.transform.position)/2f;
                    center.z = 0;
                    Vector3 camerapos = camera.transform.position;
                    camerapos.z = 0;
                    g.transform.localPosition += camerapos - center;

                    //Debug.Log(g.name + " : " + ratio[g]);
                    Vector3 _position = new Vector3(_distance.x * ratio[g].x, _distance.y * ratio[g].y, 0);
                    g.transform.position += _position;
                }
            }
            if (initialize && initialize2)
            {
                foreach (BackGroundsAndBBoxsClass _g in BackGroundsAndBBoxs)
                {
                    GameObject g = _g.BackGround;
                    Vector3 positionChange = camera.transform.position - preCameraPosition;
                    if(_g.moveBasedSpeed)
                    {
                        Vector3 _position = new Vector3(-positionChange.x * _g.speed.x, -positionChange.y * _g.speed.y);
                        g.transform.localPosition += _position;
                    }
                    else
                    {
                        Vector3 _position = new Vector3(-positionChange.x * ratio[g].x, -positionChange.y * ratio[g].y, 0);
                        g.transform.localPosition += _position;
                    }
                }
                Vector3 camerapos = camera.transform.position;
                camerapos.z = 0;
				transform.position += camerapos - preCameraPosition;
                preCameraPosition = camera.transform.position;
                preCameraPosition.z = 0;
            }
        }
    }

}