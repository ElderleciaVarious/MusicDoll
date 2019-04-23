using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MusicDoll.MasterData;

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
        /// 楽曲レベル
        /// </summary>
        [SerializeField]
        private Image levelImage = null;

        /// <summary>
        /// 楽曲難易度
        /// </summary>
        [SerializeField]
        private Image difficultyImage = null;

        /// <summary>
        /// 楽曲情報を設定する
        /// </summary>
        public void SetMusicInfo(MusicMasterData musicMaster, MusicSheetMasterData sheetMaster)
        {
            image.sprite = Resources.Load<Sprite>(musicMaster.FileName + "/image");;
            titleText.text = musicMaster.Name;
            artistText.text = musicMaster.ArtistName;

            levelImage.sprite = Resources.Load<Sprite>("Music/select/level_" + sheetMaster.Level.ToString());

            string difficulty = sheetMaster.Difficulty == MusicDifficultyKind.Astral ? "a" : "c";
            difficultyImage.sprite = Resources.Load<Sprite>("Music/select/difficulty_" + difficulty);
        }

        public void SetBpm(int bpm)
        {
            bpmText.text = bpm.ToString();
        }
    }
}