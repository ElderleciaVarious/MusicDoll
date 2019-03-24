using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicDoll
{
	/// <summary>
	/// 楽曲プレイ中のUI
	/// </summary>
	public class MusicUIManager : MonoBehaviour
	{
        /// <summary>
        /// ジャケ絵
        /// </summary>
        [SerializeField]
        private Image image = null;

        /// <summary>
        /// 曲名
        /// </summary>
        [SerializeField]
        private Text titleText = null;

        /// <summary>
        /// アーティスト名
        /// </summary>
        [SerializeField]
        private Text artistText = null;

        /// <summary>
        /// BPM
        /// </summary>
        [SerializeField]
        private Text bpmText = null;

        /// <summary>
        /// 楽曲情報を設定する
        /// </summary>
        public void SetMusicInfo(Sprite sprite, string titleName, string artistName)
        {
            image.sprite = sprite;
            titleText.text = titleName;
            artistText.text = artistName;
        }
    }
}