using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// BPM変化ごとのデータ
	/// </summary>
	public class MusicTempoData
	{
        /// <summary>
        /// 実際のBPM値
        /// </summary>
        public float Bpm { private set; get; }

        /// <summary>
        /// スケールをかけたBPM値
        /// </summary>
        public int BpmLargeScaleValue { private set; get; }

        /// <summary>
        /// 楽曲全体での配置カウント
        /// </summary>
        public int TotalPosition { private set; get; }

        /// <summary>
        /// このBPMが開始される譜面時間
        /// </summary>
		public float StartTime { private set; get; }

        public MusicTempoData(int bpmLargeScale, int measure, int position)
        {
            BpmLargeScaleValue = bpmLargeScale;
            Bpm = (float)bpmLargeScale / MusicConst.BPM_SCALE;

            TotalPosition = MusicConst.POSITION_FINENESS * measure + position;

            StartTime = 0;
        }

        /// <summary>
        /// 前回のBPM変化から譜面時間を計算する
        /// </summary>
        public void InitializeStartTime(MusicTempoData beforeTempo)
        {
            // 前回のBPM変化からこのBPM変化までのカウント
            int diffCount = TotalPosition - beforeTempo.TotalPosition;

            int BaseTemp = diffCount * 24000;
            int divTemp = MusicConst.POSITION_FINENESS * beforeTempo.BpmLargeScaleValue;

            float measureCount = (float)diffCount / MusicConst.POSITION_FINENESS;
            float bpmTime = 24000f / beforeTempo.BpmLargeScaleValue;

            StartTime = beforeTempo.StartTime + measureCount * bpmTime;
        }
    }
}