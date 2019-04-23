using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// 楽曲のテンポ変動を管理するクラス
    /// 時間に関する実計算はこのクラスではなくMusicTimeManagerかMusicTempoDataで行う
	/// </summary>
	public class MusicTempoManager
	{
        /// <summary>
        /// BPM設定が初期化されているか
        /// </summary>
        private bool isBpmInitialized;

        /// <summary>
        /// BPMの定義値
        /// </summary>
        private Dictionary<string, int> bpmDefinition = new Dictionary<string, int>();

        /// <summary>
        /// BPM変化リスト
        /// </summary>
        private List<MusicTempoData> tempoDataList = new List<MusicTempoData>();

        /// <summary>
        /// 次のテンポを表すtempoDataListのIndex
        /// </summary>
        private int nextTempoIndex = 0;

        /// <summary>
        /// 次のBPM変化がない場合に代入しておくIndex
        /// </summary>
        private static readonly int NoNextTempoIndex = -1;

        /// <summary>
        /// 現在のスケールBPM
        /// </summary>
        public int currentBpmLargeScale { private set; get; }

        /// <summary>
        /// 現在のBPMが開始された譜面時間
        /// </summary>
        public float currentBpmStartTime { private set; get; }

        /// <summary>
        /// 現在のBPMが開始された譜面カウント
        /// </summary>
        public int currentBpmStartPosition { private set; get; }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            isBpmInitialized = false;
            bpmDefinition.Clear();
            tempoDataList.Clear();
        }

        /// <summary>
        /// リスト内のテンポデータを初期化する
        /// </summary>
        public void PrecomputeTime()
        {
            // 初期化済みの場合は改めて初期化する必要はない
            if(isBpmInitialized)
            {
                return;
            }
            for (int index = 1; index < tempoDataList.Count; ++index)
            {
                MusicTempoData tempo = tempoDataList[index];
                tempo.InitializeStartTime(tempoDataList[index - 1]);
            }
        }

        /// <summary>
        /// 楽曲を始める
        /// </summary>
        public void StartMusic()
        {
            nextTempoIndex = 0;
            UpdateCheckTempo(float.MaxValue);   // 必ずIndex:0のデータに設定するため時間をMaxValueとする
        }

        /// <summary>
        /// 現在時間からテンポを更新するか判断する
        /// </summary>
        public void UpdateCheckTempo(float currentTime)
        {
            // 次のBPM変化がない場合は何もしない
            if(nextTempoIndex == NoNextTempoIndex)
            {
                return;
            }

            // 次のテンポデータの時間に到達していない場合は何もしない
            if(currentTime < tempoDataList[nextTempoIndex].StartTime)
            {
                return;
            }

            // 次のテンポデータを設定する
            currentBpmLargeScale = tempoDataList[nextTempoIndex].BpmLargeScaleValue;
            currentBpmStartTime = tempoDataList[nextTempoIndex].StartTime;
            currentBpmStartPosition = tempoDataList[nextTempoIndex].TotalPosition;
            ++nextTempoIndex;
            MusicManager.Instance.SetCurrentBpmText();
            
            // テンポのリスト末尾に到達したら次の変化はない
            if(nextTempoIndex >= tempoDataList.Count)
            {
                nextTempoIndex = NoNextTempoIndex;
            }
        }

        /// <summary>
        /// BPM定義を追加する
        /// </summary>
        public void AddBPMDefinition(string bpmKey, string bpmString)
        {
            bpmDefinition[bpmKey] = GetBpmScaleValue(bpmString);
        }

        /// <summary>
        /// 曲開始時のBPMを設定する
        /// </summary>
        public void SetStartBpm(int bpmLargeScale)
        {
            tempoDataList.Insert(0, new MusicTempoData(bpmLargeScale, 0, 0));
        }

        /// <summary>
        /// 指定場所にBPM変化を追加する
        /// </summary>
        public void AddBPMChange(string bpmKey, int measure, int position)
        {
            AddTempoDataList(bpmDefinition[bpmKey], measure, position);
        }

        /// <summary>
        /// 指定場所にBPM変化を追加する
        /// </summary>
        public void AddBPMChange(int bpm, int measure, int position)
        {
            AddTempoDataList(GetBpmScaleValue(bpm), measure, position);
        }

        /// <summary>
        /// BPM変化をテンポデータとして登録する
        /// </summary>
        private void AddTempoDataList(int bpmLargeScale, int measure, int position)
        {
            tempoDataList.Add(new MusicTempoData(bpmLargeScale, measure, position));
        }

        /// <summary>
        /// スケールをかけて整数値としたBPM値を取得する
        /// </summary>
        public static int GetBpmScaleValue(string bpmString)
        {
            return (int)(double.Parse(bpmString) * MusicConst.BPM_SCALE);
        }

        /// <summary>
        /// スケールをかけて整数値としたBPM値を取得する
        /// </summary>
        public static int GetBpmScaleValue(int originalBpm)
        {
            return originalBpm * MusicConst.BPM_SCALE;
        }
	}
}