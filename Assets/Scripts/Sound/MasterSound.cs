using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public class MasterSound : MonoBehaviour
    {
        public AudioSource IncidentalAudioSource;
        public AudioClip WindMagicApplyClip;
        public AudioClip EarthMagicApplyClip;

        public void PlayWindMagicApplySound() => PlayClip(WindMagicApplyClip);
        public void PlayEarthMagicApplySound() => PlayClip(EarthMagicApplyClip);

        private void PlayClip(AudioClip clip)
        {
            IncidentalAudioSource.clip = clip;
            IncidentalAudioSource.Play();
        }
    }
}
