using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class OutdoorsBackgroundMusic : MonoBehaviour
    {
        public List<AudioClip> audioClips;
        public AudioSource audioSource;

        void Start()
        {
            if (audioClips.Count > 0 && audioSource != null)
            {
                StartCoroutine(PlayInitialMusic());
            }
        }

        /// <summary>
        /// 첫 실행시에는 10초 후에 음악 재생. 이후 240~400초 중 랜덤한 시간에 랜덤 음악 재생
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayInitialMusic()
        {
            yield return new WaitForSeconds(10f);
            AudioClip randomClip = audioClips[Random.Range(0, audioClips.Count)];
            audioSource.PlayOneShot(randomClip);

            while (true)
            {
                float waitTime = Random.Range(540f, 700f);
                yield return new WaitForSeconds(waitTime);

                randomClip = audioClips[Random.Range(0, audioClips.Count)];
                audioSource.PlayOneShot(randomClip);
            }
        }
    }
}