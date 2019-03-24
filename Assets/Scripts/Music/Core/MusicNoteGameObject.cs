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

        [SerializeField]
        private MusicTapEffectObject effect = null;

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
        /// ノーツ画像
        /// </summary>
        public Image NoteImage { get{ return image; } }

        /// <summary>
        /// 初期設定
        /// </summary>
        public void Initialize(MusicNote note)
        {
            this.note = note;

            foreach(int timing in MusicConst.NotesTimingColor.Keys)
            {
                if(note.LocalPosition % timing == 0)
                {
                    image.color = MusicConst.NotesTimingColor[timing];
                    break;
                }
            }

            note.Place.GetTargetPosition(out startPosition, out endPosition);
            transform.position = startPosition;
        }

        /// <summary>
        /// 表示位置を移動する
        /// </summary>
        /// <param name="parameter">1:生成位置 0:判定位置 とした位置の割合パラメータ</param>
        public void Move(float parameter)
        {
            transform.position = parameter * startPosition + (1 - parameter) * endPosition;
        }

        /// <summary>
        /// このオブジェクトを破棄する
        /// 必ずMusicNote.DestroyObjectから呼び出される
        /// </summary>
        public void DestroyObject()
        {
            MusicTapEffectObject effectObject = Instantiate(effect, transform.position, Quaternion.identity);
            effectObject.transform.SetParent(transform.parent);
            Destroy(gameObject);
        }
    }
}
