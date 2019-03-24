using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicDoll
{
    /// <summary>
    /// 譜面そのもの
    /// </summary>
    public class MusicSheet
    {
        /// <summary>
        /// 譜面ID
        /// </summary>
        public ulong Id { private set; get; }

        /// <summary>
        /// BPM変化を管理するクラス
        /// </summary>
        public MusicTempoManager TempoManager { private set; get; }

        /// <summary>
        /// ロードが正常に完了しているか
        /// </summary>
        public bool IsLoadCompleted { private set; get; }

        /// <summary>
        /// 譜面に存在する全てのノーツ
        /// </summary>
        private Dictionary<MusicPlaceKind, List<MusicNote>> noteList = new Dictionary<MusicPlaceKind, List<MusicNote>>();

        /// <summary>
        /// 次に生成されるかの監視対象となるノーツのインデックス
        /// </summary>
        private Dictionary<MusicPlaceKind, int> noteTopIndexList = new Dictionary<MusicPlaceKind, int>();

        /// <summary>
        /// 現在の位置から最後までに存在するノーツ（過ぎたノーツから消されていく）
        /// </summary>
        private Dictionary<MusicPlaceKind, List<MusicNote>> activeNoteList = new Dictionary<MusicPlaceKind, List<MusicNote>>();

        /// <summary>
        /// 譜面開始時のBPM
        /// </summary>
        public int StartBpm { get; set; }

        /// <summary>
        /// 最後のノーツまで参照された場合にnoteTopIndexListに入れる定数
        /// </summary>
        private static readonly int EndOfNoteIndex = -1;

        public MusicSheet(ulong id)
        {
            Id = id;
            TempoManager = new MusicTempoManager();
        }

        /// <summary>
        /// 譜面読み込み開始時の初期化処理
        /// </summary>
        public void InitializeLoad()
        {
            IsLoadCompleted = false;
            noteList.Clear();
            activeNoteList.Clear();
            TempoManager.Initialize();

            foreach(MusicPlaceKind place in Enum.GetValues(typeof(MusicPlaceKind)))
            {
                noteList[place] = new List<MusicNote>();
            }
        }

        /// <summary>
        /// 譜面読み込み完了後や再プレイ時にプレイ開始するための初期化処理
        /// </summary>
        public void InitializePlay()
        {
            // 読み込んだノーツをアクティブなノーツとして登録
            foreach (MusicPlaceKind place in Enum.GetValues(typeof(MusicPlaceKind)))
            {
                activeNoteList[place] = new List<MusicNote>(noteList[place]);
                noteTopIndexList[place] = 0;
            }

            // 登録したBPM変化から予め時間を計算しておく
            TempoManager.SetStartBpm(StartBpm);
            TempoManager.PrecomputeTime();

            IsLoadCompleted = true;
        }

        /// <summary>
        /// 指定場所にノーツを追加する
        /// </summary>
        /// <param name="position">1小節をX分割した中のY番目のノーツはX*POSITION_FINENESS/Y</param>
        public void AddMusicNote(MusicPlaceKind place, int measure, int position)
        {
            noteList[place].Add(new MusicNote(measure, position, place));
        }

        /// <summary>
        /// BPM定義を追加する
        /// </summary>
        public void AddBPMDefinition(string bpmKey, string bpmString)
        {
            TempoManager.AddBPMDefinition(bpmKey, bpmString);
        }

        /// <summary>
        /// 指定場所にBPM変化を追加する
        /// </summary>
        public void AddBPMChange(string bpmKey, int measure, int position)
        {
            TempoManager.AddBPMChange(bpmKey, measure, position);
        }

        /// <summary>
        /// 指定場所にBPM変化を追加する
        /// </summary>
        public void AddBPMChange(int bpm, int measure, int position)
        {
            TempoManager.AddBPMChange(bpm, measure, position);
        }

        /// <summary>
        /// 指定された位置で新しく出現するノーツを計算し取得する
        /// 対象となるノーツがない場合はnullを返す
        /// </summary>
        public List<MusicNote> CheckNewAppearNotes(int position)
        {
            List<MusicNote> appearNotes = null;

            foreach(MusicPlaceKind kind in Enum.GetValues(typeof(MusicPlaceKind)))
            {
                // 先頭ノーツがない場合は何もしない
                if(noteTopIndexList[kind] == EndOfNoteIndex)
                {
                    continue; 
                }

                // 次のノーツが出現位置を越えた場合は生成する
                MusicNote note = noteList[kind][noteTopIndexList[kind]];
                if(position > note.GlobalPosition)
                {
                    // 出現リストにこのノーツを追加する
                    if(appearNotes == null)
                    {
                        appearNotes = new List<MusicNote>();
                    }
                    appearNotes.Add(note);
                    activeNoteList[kind].Add(note);

                    // 次のノーツへ移動し末端に到達しら末端定数を入れる
                    ++noteTopIndexList[kind];
                    if(noteTopIndexList[kind] >= noteList[kind].Count)
                    {
                        noteTopIndexList[kind] = EndOfNoteIndex;
                    }
                }
            }

            return appearNotes;
        }

        /// <summary>
        /// 画面に表示されているノーツを指定位置へ移動する
        /// </summary>
        public void MoveActiveNotes(int position, int displayCount)
        {
            // 過ぎ去ったことにより削除されるノーツ
            List<MusicNote> destroyNoteList = null;

            foreach(MusicPlaceKind kind in Enum.GetValues(typeof(MusicPlaceKind)))
            {
                // 各ノーツを調べる
                foreach(MusicNote note in activeNoteList[kind])
                {
                    // 生成されていないノーツを参照した場合は次のレーンへ
                    if(!note.IsActive)
                    {
                        break;
                    }

                    // ノーツが過ぎ去っているかどうかを判定する
                    if (position > note.GlobalPosition)
                    {
                        note.DestroyObject();
                        if(destroyNoteList == null)
                        {
                            destroyNoteList = new List<MusicNote>();
                        }
                        destroyNoteList.Add(note);
                    }
                    else
                    {
                        note.Move(position, displayCount);
                    }
                }

                // ノーツを削除する
                if(destroyNoteList != null)
                {
                    activeNoteList[kind].RemoveAll(note => destroyNoteList.Contains(note));
                }
            }

            // 削除したノーツに応じて判定する
            if(destroyNoteList != null)
            {
                MusicManager.Instance.NoteJudge(destroyNoteList, MusicJudgeKind.PerfectGreat);
            }
        }

        /// <summary>
        /// 総ノーツ数を取得する
        /// </summary>
        public int GetTotalNotesCount()
        {
            int totalNotesCount = 0;

            foreach(MusicPlaceKind kind in Enum.GetValues(typeof(MusicPlaceKind)))
            {
                totalNotesCount += noteList[kind].Count;
            }

            return totalNotesCount;
        }
    }
}