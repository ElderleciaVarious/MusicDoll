﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll.MasterData
{
	/// <summary>
	/// 楽曲マスターデータ
	/// </summary>
	public class MusicMasterData : AMasterData
	{
        /// <summary>
        /// 作曲者名
        /// </summary>
		public string ArtistName { private set; get; }

        /// <summary>
        /// データのファイル名
        /// </summary>
        public string FileName { private set; get; }

        /// <summary>
        /// 楽曲オフセット
        /// </summary>
        public float Offset { private set; get; }

        /// <summary>
        /// 読み込んだ文字列配列から初期化する
        /// </summary>
        public override void Initialize(string[] data)
        {
            base.Initialize(data);

            ArtistName = data[2];
            FileName = data[3];
            Offset = float.Parse(data[4]);
        }

        public static MusicMasterData GetDebugSheet(ulong id)
        {
            MusicMasterData master = new MusicMasterData();

            string[] data = new string[5];
            data[0] = id.ToString();

            if(id == 1)
            {
                data[1] = "Go Beyond!!";
                data[2] = "Ryu☆ Vs. Sota";
                data[3] = "GoBeyond";
                data[4] = "0.42";
            }
            else
            {
                data[1] = "POSSESSION";
                data[2] = "TAG underground";
                data[3] = "Possession";
                data[4] = "0";
            }

            master.Initialize(data);
            return master;
        }
    }
}