using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// 音声再生の管理クラス
	/// </summary>
	public class MusicSoundManager : MonoBehaviour
	{
        /// <summary>
        /// 楽曲再生
        /// </summary>
        [SerializeField]
        private AudioSource audioBGMSource = null;

        /// <summary>
        /// SE再生
        /// </summary>
        [SerializeField]
        private AudioSource audioSESource = null;

        /// <summary>
        /// タップ時のSE
        /// </summary>
        [SerializeField]
        private AudioClip tapSE = null;

        /// <summary>
        /// 楽曲
        /// </summary>
        private AudioClip bgm = null;

        /// <summary>
        /// 使用する楽曲を読み込む
        /// </summary>
        public void LoadBGM(string fileName)
        {
            AudioClip clip = Resources.Load<AudioClip>(string.Format("{0}/music", fileName));
            audioBGMSource.clip = clip;
        }

        /// <summary>
        /// 楽曲を再生する
        /// </summary>
        public void PlayBGM()
        {
            audioBGMSource.Play();
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        public void PlaySE()
        {
            audioSESource.PlayOneShot(tapSE);
        }
	}
}