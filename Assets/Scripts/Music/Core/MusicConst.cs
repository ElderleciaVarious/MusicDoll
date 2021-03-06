﻿using System.Collections.Generic;
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
        /// ノーツカラー
        /// </summary>
        public static readonly List<Color> NotesColor = new List<Color>
        {
            new Color(1.0f, 0.3f, 0.3f),
            new Color(1.0f, 1.0f, 0.3f),
            new Color(0.3f, 1.0f, 0.3f),
            new Color(0.3f, 1.0f, 1.0f),
            new Color(0.3f, 0.3f, 1.0f),
            new Color(1.0f, 0.3f, 1.0f),
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
        public static readonly int TotalGaugeValue = 3000;

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
        CenterLower1,
        CenterLower2,
        CenterLower3,
        CenterLower4,
        CenterLower5,
        CenterUpper1,
        CenterUpper2,
        CenterUpper3,
        CenterUpper4,
        CenterUpper5,
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