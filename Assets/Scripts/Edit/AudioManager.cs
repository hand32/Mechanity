using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace roguelike
{
    public class AudioManager : MonoBehaviour
    {

        public bool playOnSceneStart;
        public bool randomPlayOnStart;
        public List<Sound> sounds;

        int random;

        void Awake()
        {
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
                s.source.playOnAwake = false;
                if(playOnSceneStart && !randomPlayOnStart)
                {
                    Play(s.name);
                }
            }
            if(playOnSceneStart && !randomPlayOnStart)
            {
                Play(sounds[Random.Range(0, sounds.Count)].name);
            }
        }

        string RandomSelect(string name)
        {
            List<Sound> _list = new List<Sound>();
            foreach(Sound _s in sounds)
            {
                if(_s.name.Contains(name) && _s.name.Length == name.Length + 2)
                {
                    _list.Add(_s);
                }
            }

            return _list[Random.Range(0, _list.Count)].name;
        }

        public void RandomPlay(string name)
        {
            Play(RandomSelect(name));
        }

        public void Play(string name)
        {
            foreach (Sound s in sounds)
            {
                if (s.name == name)
                {
                    s.source.Play();
                    break;
                }
            }
        }

        public void Stop(string name)
        {
            foreach (Sound s in sounds)
            {
                if (s.name == name)
                {
                    s.source.Stop();
                    break;
                }
            }
        }

    }

}