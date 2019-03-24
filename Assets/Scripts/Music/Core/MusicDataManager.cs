using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// 譜面データの管理クラス
	/// </summary>
	public class MusicDataManager
	{
        /// <summary>
        /// 現在の楽曲の譜面データ
        /// </summary>
        public MusicSheet currentSheet { private set; get; }

        /// <summary>
        /// 譜面データのキャッシュ
        /// </summary>
        private Dictionary<ulong, MusicSheet> musicSheetCache = new Dictionary<ulong, MusicSheet>();

        /// <summary>
        /// 譜面データを読み込み設定する
        /// キャッシュに残っている場合はキャッシュから取得する
        /// </summary>
        public void LoadMusicSheet(ulong sheetId)
        {
            MusicSheet sheet = null;

            // キャッシュを確認しない場合は新しく読み込む
            if(musicSheetCache.ContainsKey(sheetId))
            {
                sheet = musicSheetCache[sheetId];
            }
            else
            {
                MasterData.MusicMasterData master = MasterData.MusicMasterData.GetDebugSheet(sheetId);
                sheet = BMSLoader.Load(master);
                musicSheetCache.Add(sheetId, sheet);
            }

            currentSheet = sheet;
        }
    }
}