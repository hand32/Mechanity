using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{

    float duration;
    float magnitude;
    float elapsed;
    Vector2 dir;
    bool shaking;

    Dictionary<float, float> shakeQueue = new Dictionary<float, float>();

    CinemachineVirtualCamera virtualCamera;


    void OnEnable()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void OnGUI()
    {
        if (shaking)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        }
    }



    public void Shake(float _duration, float _magnitude)
    {
        try
        {
            if (!shaking)
            {
                duration = _duration;
                magnitude = _magnitude;
                shakeQueue.Clear();
                shakeQueue.Add(_duration, _magnitude);
                StartCoroutine("ShakeIt");
            }
            else
            {
                //  다음에 하기.
                if (duration - elapsed >= _duration)
                {
                    duration = elapsed + _duration;
                }
                magnitude += Mathf.Log10(magnitude + 10) * _magnitude;

                if (shakeQueue.ContainsKey(elapsed + _duration))
                    shakeQueue[elapsed + _duration] += _magnitude;
                else
                    shakeQueue.Add(elapsed + _duration, _magnitude);
            }
        }

        catch
        {
            Debug.Log("Shake Dictionary Error!");
            duration = 0f;
            magnitude = 0f;
            elapsed = 0f;
            shaking = false;
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
            shakeQueue.Clear();
        }
    }

    IEnumerator ShakeIt()
    {
        elapsed = 0.0f;
        shaking = true;

        while (elapsed <= duration)
        {
            if (elapsed >= 25f)
            {
                Debug.Log("Too long Shaking!");
                shakeQueue.Clear();
                duration = 0f;
                magnitude = 0f;
                elapsed = 0f;
                virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
                shaking = false;
                break;
            }
            elapsed += Time.deltaTime;
            List<float> keys = new List<float>(shakeQueue.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                float key = keys[i];

                if (key <= elapsed && shakeQueue[key] > 0f)
                {
                    magnitude -= Mathf.Log10(magnitude + 10) * shakeQueue[key];
                    shakeQueue.Remove(key);
                }
            }
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = magnitude;
            yield return null;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        }
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        shakeQueue.Clear();
        shaking = false;

    }
}
