using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// Music Mainシーン
	/// </summary>
	public class MusicMainScene : MonoBehaviour
	{
        /// <summary>
        /// デバッグ用：プレイする譜面ID
        /// </summary>
        [SerializeField]
        private ulong debugMusicSheetId = 0;

        public void Start()
        {
            MusicManager.Instance.LoadMusicSheet(debugMusicSheetId);
        }
    }
}