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

        public static MusicSheetMasterData GetDebugSheet(ulong id)
        {
            MusicSheetMasterData master = new MusicSheetMasterData();

            string[] data = new string[7];
            data[0] = id.ToString();

            if(id == 3)
            {
                data[2] = "3";
                data[3] = "3";
                data[4] = "7";
                data[5] = "206";
                data[6] = "206";
            }
            else if(id == 4)
            {
                data[2] = "4";
                data[3] = "1";
                data[4] = "4";
                data[5] = "279";
                data[6] = "279";
            }
            else
            {
                data[2] = id.ToString();
                data[3] = "3";
                data[4] = "9";
                data[5] = "200";
                data[6] = "200";
            }

            master.Initialize(data);
            return master;
        }
    }
}