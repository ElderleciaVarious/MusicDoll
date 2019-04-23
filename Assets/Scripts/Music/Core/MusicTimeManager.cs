using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
    /// <summary>
    /// 譜面進行やタイミング判定といったプレイ中の時間を管理するクラス
    /// </summary>
    public class MusicTimeManager : MonoBehaviour
    {
#if DEBUG
        /// <summary>
        /// デバッグ用：タイミングをずらす時間
        /// </summary>
        [SerializeField]
        private float debugTimeOffset = 0f;
#endif

        /// <summary>
        /// 譜面開始時点でのタイマー時間
        /// </summary>
        private float musicStartTime;

        /// <summary>
        /// 譜面開始から呼び出し時点までの経過時間
        /// </summary>
        public float MusicTimer { private set; get; }

        /// <summary>
        /// 呼び出し時点の譜面カウント
        /// </summary>
        public int MusicPosition { private set; get; }

        /// <summary>
        /// テンポ管理クラス
        /// </summary>
        private MusicTempoManager tempoManager;

        /// <summary>
        /// 初期化完了フラグ
        /// </summary>
        private bool isInitialized = false;

        private void Update()
        {
            if(!isInitialized)
            {
                return;
            }

            MusicTimer = Time.time - musicStartTime;
#if DEBUG
            MusicTimer -= debugTimeOffset;
#endif

            tempoManager.UpdateCheckTempo(MusicTimer);

            float diffTime = MusicTimer - tempoManager.currentBpmStartTime;
            int positionCount = (int)(diffTime * tempoManager.currentBpmLargeScale / 24000f * MusicConst.POSITION_FINENESS);

            MusicPosition = tempoManager.currentBpmStartPosition + positionCount;
        }

        /// <summary>
        /// プレイ開始設定を行う
        /// </summary>
        public void StartMusicTimer(float timerOffset)
        {
            musicStartTime = Time.time + timerOffset;
            tempoManager = MusicManager.Instance.GetCurrentTempoManager();
            tempoManager.StartMusic();

            isInitialized = true;
        }
    }
}
