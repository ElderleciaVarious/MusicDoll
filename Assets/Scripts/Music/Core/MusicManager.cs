﻿using UnityEngine;
using System.Collections.Generic;

namespace MusicDoll
{
    /// <summary>
    /// 楽曲プレイ全体を管理するクラス
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        /// <summary>
        /// 1画面に生成するノーツの譜面カウント
        /// </summary>
        [SerializeField]
        private int debugNotesDisplay = 1000;

        /// <summary>
        /// 譜面時間管理クラス
        /// </summary>
        [SerializeField]
        private MusicTimeManager timeManager = null;

        /// <summary>
        /// ノーツオブジェクト管理クラス
        /// </summary>
        [SerializeField]
        private MusicNoteGameObjectManager objectManager = null;

        /// <summary>
        /// 楽曲スコア管理クラス
        /// </summary>
        [SerializeField]
        private MusicScoreManager scoreManager = null;

        /// <summary>
        /// UI管理クラス
        /// </summary>
        [SerializeField]
        private MusicUIManager uiManager = null;

        /// <summary>
        /// サウンド管理クラス
        /// </summary>
        [SerializeField]
        MusicSoundManager soundManager = null;

        [SerializeField]
        private MusicTapLineObject tapLinePrefab = null;

        [SerializeField]
        private Transform lineRoot = null;

        /// <summary>
        /// MusicManagerインスタンス
        /// Awakeで初期化されるため、MusicMainシーン内のStart以降でのみ呼び出せる
        /// </summary>
        public static MusicManager Instance { private set; get; }

        /// <summary>
        /// 譜面データ管理クラス
        /// </summary>
        private MusicDataManager dataManager = new MusicDataManager();

        /// <summary>
        /// プレイ中の状態
        /// </summary>
        private enum State
        {
            Init,
            Start,
            Playing
        }
        private State state = State.Init;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            switch(state)
            {
                case State.Init:
                    break;
                case State.Start:
                    MasterData.MusicMasterData master = MasterData.MusicMasterData.GetDebugSheet(dataManager.currentSheet.Id);
                    timeManager.StartMusicTimer(master.Offset);
                    soundManager.PlayBGM();
                    state = State.Playing;
                    break;
                case State.Playing:
                    UpdateMusicNotes();
                    break;
            }
        }

        /// <summary>
        /// 毎フレームの譜面位置を監視し生成位置に達したノーツを生成する
        /// </summary>
        private void UpdateMusicNotes()
        {
            // ハイスピードにより算出された1画面表示量
            int displayCount = 0;
#if DEBUG
            displayCount += debugNotesDisplay;
#endif
            List<MusicNote> appearNotes = dataManager.currentSheet.CheckNewAppearNotes(timeManager.MusicPosition + displayCount);

            // 出現ノーツがある場合は出現処理
            if(appearNotes != null)
            {
                // 手前で出現したノーツ
                MusicNote beforeNote = null;

                foreach(MusicNote note in appearNotes)
                {
                    // ノーツ生成処理
                    objectManager.GenerateObject(note);

                    // 同時押し線表示処理
                    if (beforeNote != null)
                    {
                        MusicTapLineObject line = Instantiate(tapLinePrefab, lineRoot);
                        line.Initialize(beforeNote.NoteObject.gameObject, note.NoteObject.gameObject);
                    }
                    beforeNote = note;
                }
            }

            // 全てのノーツを動かす
            dataManager.currentSheet.MoveActiveNotes(timeManager.MusicPosition, displayCount);
        }

        /// <summary>
        /// 使用する譜面を登録する
        /// </summary>
        public void LoadMusicSheet(ulong sheetId)
        {
            dataManager.LoadMusicSheet(sheetId);

            // 楽曲情報を設定する
            MasterData.MusicMasterData master = MasterData.MusicMasterData.GetDebugSheet(sheetId);
            Sprite sprite = Resources.Load<Sprite>(master.FileName + "/image");
            uiManager.SetMusicInfo(sprite, master.Name, master.ArtistName);

            // 譜面の初期設定を行う
            if(dataManager.currentSheet.IsLoadCompleted)
            {
                dataManager.currentSheet.InitializePlay();
            }

            scoreManager.Initialize(dataManager.currentSheet.GetTotalNotesCount());
            soundManager.LoadBGM(master.FileName);

            state = State.Start;
        }

        /// <summary>
        /// 指定のノーツを指定の判定で取る
        /// </summary>
        public void NoteJudge(List<MusicNote> notes, MusicJudgeKind kind)
        {
            foreach(MusicNote note in notes)
            {
                scoreManager.AddJudge(note, kind);
            }
            soundManager.PlaySE();
        }

        /// <summary>
        /// 現在の譜面のMusicTempoManagerを取得する
        /// </summary>
        public MusicTempoManager GetCurrentTempoManager()
        {
            return dataManager.currentSheet.TempoManager;
        }
    }
}