using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// ノーツ位置を指定するロケーターオブジェクトを管理するクラス
	/// </summary>
	public class MusicTapNotesLocator : MonoBehaviour
	{
        [SerializeField] private RectTransform leftUpper = null;
        [SerializeField] private RectTransform leftLower = null;
        [SerializeField] private RectTransform rightUpper = null;
        [SerializeField] private RectTransform rightLower = null;
		[SerializeField] private RectTransform centerLowerLeft = null;
		[SerializeField] private RectTransform centerLowerRight = null;
		[SerializeField] private RectTransform centerUpperLeft = null;
		[SerializeField] private RectTransform centerUpperRight = null;

		/// <summary>
		/// ノーツの開始位置
		/// </summary>
		public Dictionary<MusicPlaceKind, Vector3> StartPositions { get; private set; }

		/// <summary>
		/// ノーツの終了位置
		/// </summary>
		public Dictionary<MusicPlaceKind, Vector3> EndPositions { get; private set; }

        /// <summary>
        /// ノーツ位置から判定文字位置インデックスを返す
        /// </summary>
        public Dictionary<MusicPlaceKind, int> PlaceToJudgePosition { get; private set; }

        /// <summary>
        /// インスタンス
        /// </summary>
		public static MusicTapNotesLocator Instance { private set; get; }

		private void Awake()
		{
            Initialize();
		}

        private void Initialize()
        {
            Instance = this;
            StartPositions = new Dictionary<MusicPlaceKind, Vector3>();
            EndPositions = new Dictionary<MusicPlaceKind, Vector3>();
            PlaceToJudgePosition = new Dictionary<MusicPlaceKind, int>();

            // 両サイドの判定ラインの位置を設定
            StartPositions[MusicPlaceKind.LeftLower] = rightLower.anchoredPosition;
            EndPositions[MusicPlaceKind.LeftLower] = leftLower.anchoredPosition;
            StartPositions[MusicPlaceKind.LeftUpper] = rightUpper.anchoredPosition;
            EndPositions[MusicPlaceKind.LeftUpper] = leftUpper.anchoredPosition;
            StartPositions[MusicPlaceKind.RightLower] = leftLower.anchoredPosition;
            EndPositions[MusicPlaceKind.RightLower] = rightLower.anchoredPosition;
            StartPositions[MusicPlaceKind.RightUpper] = leftUpper.anchoredPosition;
            EndPositions[MusicPlaceKind.RightUpper] = rightUpper.anchoredPosition;

            // 上と下の判定ラインの位置を計算
            for(int i = 0; i < 5; ++i)
            {
                MusicPlaceKind lowerKind = MusicPlaceKind.CenterLower1 + i;
                MusicPlaceKind upperKind = MusicPlaceKind.CenterUpper1 + i;

                Vector2 upperPosition = (centerUpperLeft.anchoredPosition * (4 - i) + centerUpperRight.anchoredPosition * i) / 4f;
                Vector2 lowerPosition = (centerLowerLeft.anchoredPosition * (4 - i) + centerLowerRight.anchoredPosition * i) / 4f;

                StartPositions[lowerKind] = upperPosition;
                EndPositions[lowerKind] = lowerPosition;

                StartPositions[upperKind] = lowerPosition;
                EndPositions[upperKind] = upperPosition;
            }

            // 判定文字ディクショナリを生成
            foreach(MusicPlaceKind kind in Enum.GetValues(typeof(MusicPlaceKind)))
            {
                if(kind.IsLeftSide())
                {
                    PlaceToJudgePosition[kind] = 0;
                }
                else if(kind.IsRightSide())
                {
                    PlaceToJudgePosition[kind] = 2;
                }
                else
                {
                    PlaceToJudgePosition[kind] = 1;
                }
            }
        }
    }
}