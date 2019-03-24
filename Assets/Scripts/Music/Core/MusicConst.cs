using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
    /// <summary>
    /// 楽曲プレイ中に関する定数
    /// </summary>
    public class MusicConst
    {
        /// <summary>
        /// 1小節を分割する細かさ
        /// 128×3×5=1920のため、128分や48分が表現可能
        /// </summary>
        public static readonly int POSITION_FINENESS = 1920;

        /// <summary>
        /// BPMを整数値として扱うためのスケール
        /// </summary>
        public static readonly int BPM_SCALE = 100;

        /// <summary>
        /// タイミングごとのノーツカラー
        /// </summary>
        public static readonly Dictionary<int, Color> NotesTimingColor = new Dictionary<int, Color>()
        {
            { POSITION_FINENESS /  4, Color.red },
            { POSITION_FINENESS /  8, Color.blue },
            { POSITION_FINENESS / 16, Color.yellow },
            { POSITION_FINENESS / 32, Color.green },
            { POSITION_FINENESS /  3, Color.cyan },
            { POSITION_FINENESS /  6, Color.magenta },
        };

        /// <summary>
        /// ゲージの内部数値最大値
        /// </summary>
        public static readonly int GaugeMaxValue = 1280;

        /// <summary>
        /// ゲージの粒数
        /// </summary>
        public static readonly int GaugeMaxCount = 20;

        /// <summary>
        /// ゲージ1粒あたりが持つ内部数値
        /// </summary>
        public static readonly int GaugeValuePerOne = GaugeMaxValue / GaugeMaxCount;

        /// <summary>
        /// 全てGreatで楽曲を終了した際に得られるゲージの内部数値の合計値
        /// </summary>
        public static readonly int TotalGaugeValue = 2560;

        /// <summary>
        /// 楽曲開始時のゲージ数値
        /// </summary>
        public static readonly int GaugeStartValue = GaugeValuePerOne * 6;

        /// <summary>
        /// 楽曲クリアに必要なゲージ粒数
        /// </summary>
        public static readonly int GaugeClearCount = 14;
    }

    /// <summary>
    /// 配置レーン
    /// </summary>
    public enum MusicPlaceKind
    {
        LeftUpper = 1,
        LeftLower,
        Center1,
        Center2,
        Center3,
        Center4,
        Center5,
        RightLower,
        RightUpper,
    }

    /// <summary>
    /// 判定種別
    /// </summary>
    public enum MusicJudgeKind
    {
        Bad,
        FastGood,
        SlowGood,
        FastGreat,
        SlowGreat,
        PerfectGreat
    }

    public static class MusicPlaceKindExt
    {
        /// <summary>
        /// ノーツの場所を取得する
        /// </summary>
        public static void GetTargetPosition(this MusicPlaceKind place, out Vector2 startPosition, out Vector2 endPosition)
        {
            float startX = 0f;
            float startY = 0f;
            float endX   = 0f;
            float endY   = 0f;

            if(place.IsLeftSide())
            {
                startX = 0;
                endX = -600f;
                startY = endY = (place == MusicPlaceKind.LeftUpper) ? 120f : -120f;
            }
            else if(place.IsRightSide())
            {
                startX = 0;
                endX = 600f;
                startY = endY = (place == MusicPlaceKind.RightUpper) ? 120f : -120f;
            }
            else
            {
                startY = 680f;
                endY = -400f;

                startX = endX = 200f * (int)place - 1000f;
            }

            startPosition = new Vector2(startX, startY);
            endPosition = new Vector2(endX, endY);
        }

        /// <summary>
        /// 左側ノーツの場合
        /// </summary>
        public static bool IsLeftSide(this MusicPlaceKind place)
        {
            return place == MusicPlaceKind.LeftLower || place == MusicPlaceKind.LeftUpper;
        }

        /// <summary>
        /// 右側ノーツの場合
        /// </summary>
        public static bool IsRightSide(this MusicPlaceKind place)
        {
            return place == MusicPlaceKind.RightLower || place == MusicPlaceKind.RightUpper;
        }
    }
}