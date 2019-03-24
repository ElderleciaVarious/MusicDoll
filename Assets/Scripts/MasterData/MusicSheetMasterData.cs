using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll.MasterData
{
	/// <summary>
	/// 譜面マスターデータ
	/// </summary>
	public class MusicSheetMasterData : AMasterData
	{
        /// <summary>
        /// 対応する楽曲のID
        /// </summary>
        public ulong MusicId { private set; get; }

        /// <summary>
        /// 譜面難易度
        /// </summary>
		public MusicDifficultyKind Difficulty { private set; get; }

        /// <summary>
        /// 譜面レベル
        /// </summary>
        public int Level { private set; get; }

        /// <summary>
        /// 最大BPM(表示用)
        /// </summary>
        public int MaxBpm { private set; get; }

        /// <summary>
        /// 最小BPM(表示用)
        /// </summary>
        public int MinBpm { private set; get; }

        /// <summary>
        /// 読み込んだ文字列配列から初期化する
        /// </summary>
        public override void Initialize(string[] data)
        {
            base.Initialize(data);

            MusicId = ulong.Parse(data[2]);
            Difficulty = (MusicDifficultyKind)int.Parse(data[3]);
            Level = int.Parse(data[4]);
            MaxBpm = int.Parse(data[5]);
            MinBpm = int.Parse(data[6]);
        }
    }
}