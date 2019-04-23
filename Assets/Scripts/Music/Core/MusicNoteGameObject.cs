using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicDoll
{
    /// <summary>
    /// 1つのタップオブジェクト
    /// </summary>
    public class MusicNoteGameObject : MonoBehaviour
    {
        /// <summary>
        /// ノーツ画像
        /// </summary>
        [SerializeField]
        private Image image = null;

        /// <summary>
        /// ノーツデータ
        /// </summary>
        private MusicNote note;

        /// <summary>
        /// 開始場所
        /// </summary>
        private Vector2 startPosition;

        /// <summary>
        /// 終了場所
        /// </summary>
        private Vector2 endPosition;

        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// ノーツ画像
        /// </summary>
        public Image NoteImage { get{ return image; } }

        /// <summary>
        /// ノーツ色
        /// </summary>
        private Color noteColor;

        /// <summary>
        /// ノーツ色
        /// </summary>
        public Color NoteColor { get { return noteColor; }
            set
            {
                noteColor = value;
                NoteImage.color = noteColor;
            } }

        /// <summary>
        /// 現在使用中のノーツであるか
        /// </summary>
        public bool IsActive { private set; get; }

        /// <summary>
        /// 対応するノーツがすでに過ぎ去っているか
        /// </summary>
        public bool IsNotePassed { get { return note.IsAppeared && !note.IsActive; } }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        public void Initialize(MusicNote note)
        {
            this.note = note;

            startPosition = MusicTapNotesLocator.Instance.StartPositions[note.Place];
            endPosition = MusicTapNotesLocator.Instance.EndPositions[note.Place];
            rectTransform.anchoredPosition = startPosition;
            IsActive = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 表示位置を移動する
        /// </summary>
        /// <param name="parameter">1:生成位置 0:判定位置 とした位置の割合パラメータ</param>
        public void Move(float parameter)
        {
            rectTransform.anchoredPosition = parameter * startPosition + (1 - parameter) * endPosition;

            noteColor.a = 1f - parameter * parameter * parameter;
            image.color = NoteColor;
        }

        /// <summary>
        /// このオブジェクトの使用を終えて非表示にする
        /// </summary>
        public void Stop()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }
    }
}
