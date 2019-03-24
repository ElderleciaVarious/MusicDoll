using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicDoll
{
	/// <summary>
	/// プレイ中のスコアやゲージに関する管理クラス
	/// </summary>
	public class MusicScoreManager : MonoBehaviour
	{
        [SerializeField]
        private GameObject gaugePrefab = null;

        /// <summary>
        /// スコア表示テキスト
        /// </summary>
        [SerializeField]
        private Text scoreText = null;

        /// <summary>
        /// G-Great表示テキスト
        /// </summary>
        [SerializeField]
        private Text gGreatText = null;

        /// <summary>
        /// S-Great表示テキスト
        /// </summary>
        [SerializeField]
        private Text sGreatText = null;

        /// <summary>
        /// Fast-Good表示テキスト
        /// </summary>
        [SerializeField]
        private Text fastText = null;

        /// <summary>
        /// Slow-Good表示テキスト
        /// </summary>
        [SerializeField]
        private Text slowText = null;

        /// <summary>
        /// Bad表示テキスト
        /// </summary>
        [SerializeField]
        private Text badText = null;

        /// <summary>
        /// ゲージ表示のルート
        /// </summary>
        [SerializeField]
        private Transform gaugeRoot = null;

        /// <summary>
        /// 判定表示オブジェクト
        /// </summary>
        [SerializeField]
        private MusicJudgeObject[] judgeObjects = null;

        /// <summary>
        /// ゲージ表示オブジェクト
        /// </summary>
        private List<Image> gaugeObjects = new List<Image>();

        /// <summary>
        /// ゲージ残量
        /// </summary>
        public int GaugeValue { private set; get; }

        /// <summary>
        /// スコア
        /// </summary>
        public int ScoreValue { private set; get; }

        /// <summary>
        /// コンボ
        /// </summary>
        public int ComboValue { private set; get; }

        /// <summary>
        /// 判定数
        /// </summary>
        public Dictionary<MusicJudgeKind, int> JudgeCount { private set; get; }

        /// <summary>
        /// 現在のゲージ粒数　表示更新時にここの値は更新される
        /// </summary>
        private int beforeGaugeCount;

        /// <summary>
        /// 総ノーツ数
        /// </summary>
        private int totalNotesCount;

        /// <summary>
        /// 判定ごとのゲージ増減数
        /// </summary>
        private Dictionary<MusicJudgeKind, int> judgeGaugeValues = new Dictionary<MusicJudgeKind, int>();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(int totalNotesCount)
        {
            // 総ノーツ数から計算
            this.totalNotesCount = totalNotesCount;
            InitializeGaugeValue();

            // スコア
            ScoreValue = 0;
            scoreText.text = "0";
            ComboValue = 0;

            // 判定
            JudgeCount = new Dictionary<MusicJudgeKind, int>();
            foreach(MusicJudgeKind kind in Enum.GetValues(typeof(MusicJudgeKind)))
            {
                JudgeCount.Add(kind, 0);
            }
            gGreatText.text = "0";

            // ゲージ
            for (int index = 0; index < MusicConst.GaugeMaxCount; ++index)
            {
                GameObject obj = Instantiate(gaugePrefab, gaugeRoot) as GameObject;
                Image objImage = obj.GetComponent<Image>();
                gaugeObjects.Add(objImage);

                objImage.color = (index < MusicConst.GaugeClearCount) ? Color.green : Color.red;
            }

            GaugeValue = MusicConst.GaugeStartValue;
            beforeGaugeCount = MusicConst.GaugeMaxCount;

            RefreshGauge();
        }

        /// <summary>
        /// 指定の判定スコアを追加する
        /// </summary>
        public void AddJudge(MusicNote note, MusicJudgeKind kind)
        {
            ++JudgeCount[kind];
            gGreatText.text = JudgeCount[kind].ToString();

            RefreshScore();

            GaugeValue += judgeGaugeValues[kind];
            if(GaugeValue > MusicConst.GaugeMaxValue)
            {
                GaugeValue = MusicConst.GaugeMaxValue;
            }
            RefreshGauge();

            // コンボ
            if(kind == MusicJudgeKind.Bad)
            {
                ComboValue = 0;
            }
            else
            {
                ++ComboValue;
            }

            // 判定表示
            judgeObjects[((int)note.Place - 1) / 3].Set(kind, ComboValue);
        }

        /// <summary>
        /// 判定ごとのゲージ増減数を設定する
        /// </summary>
        private void InitializeGaugeValue()
        {
            int greatGaugeValue = MusicConst.TotalGaugeValue / totalNotesCount;
            int goodGaugeValue  = MusicConst.TotalGaugeValue / totalNotesCount / 2;
            if(goodGaugeValue <= 0)
            {
                goodGaugeValue = 1;
            }

            judgeGaugeValues[MusicJudgeKind.PerfectGreat] = greatGaugeValue;
            judgeGaugeValues[MusicJudgeKind.FastGreat] = greatGaugeValue;
            judgeGaugeValues[MusicJudgeKind.SlowGreat] = greatGaugeValue;
            judgeGaugeValues[MusicJudgeKind.FastGood] = goodGaugeValue;
            judgeGaugeValues[MusicJudgeKind.SlowGood] = goodGaugeValue;
            judgeGaugeValues[MusicJudgeKind.Bad] = -40;
        }

        /// <summary>
        /// スコアを再計算する
        /// </summary>
        private void RefreshScore()
        {
            int judgeRate = JudgeCount[MusicJudgeKind.PerfectGreat] * 100
                + JudgeCount[MusicJudgeKind.FastGreat] * 99
                + JudgeCount[MusicJudgeKind.SlowGreat] * 99
                + JudgeCount[MusicJudgeKind.FastGood] * 70
                + JudgeCount[MusicJudgeKind.SlowGood] * 70;

            ScoreValue = 10000 * judgeRate / totalNotesCount;
            scoreText.text = ScoreValue.ToString();
        }

        /// <summary>
        /// ゲージ残量に応じて表示を更新する
        /// </summary>
        private void RefreshGauge()
        {
            // ゲージの粒数
            int gaugeCount = GaugeValue / MusicConst.GaugeValuePerOne;

            // 更新範囲を設定する
            int minIndex = gaugeCount;
            int maxIndex = beforeGaugeCount;
            if(gaugeCount > beforeGaugeCount)
            {
                minIndex = beforeGaugeCount;
                maxIndex = gaugeCount;
            }

            // 表示を更新する
            for (int index = minIndex; index < maxIndex; ++index)
            {
                Color color = gaugeObjects[index].color;
                color.a = (index < gaugeCount) ? 1.0f : 0.3f;
                gaugeObjects[index].color = color;
            }

            // 次回の表示更新の処理を減らすためゲージ粒数を保存する
            beforeGaugeCount = gaugeCount;
        }
    }
}