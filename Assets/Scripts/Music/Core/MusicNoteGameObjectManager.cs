using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MusicDoll
{
	/// <summary>
	/// ノーツのGameObject全体を管理するクラス
	/// </summary>
	public class MusicNoteGameObjectManager : MonoBehaviour
	{
        /// <summary>
        /// ノーツのプレハブ
        /// </summary>
        [SerializeField]
        private MusicNoteGameObject noteObjectPrefab = null;

        /// <summary>
        /// ノーツ同時押し線のプレハブ
        /// </summary>
        [SerializeField]
        private MusicTapLineObject tapLinePrefab = null;

        /// <summary>
        /// エフェクトプレハブ
        /// </summary>
        [SerializeField]
        private MusicTapEffectObject effectPrefab = null;

        /// <summary>
        /// ノーツ生成ルート
        /// </summary>
        [SerializeField]
        private Transform noteObjectRoot = null;

        [SerializeField]
        private Sprite[] noteSprites = null;

        /// <summary>
        /// ノーツオブジェクト群
        /// </summary>
        private List<MusicNoteGameObject> generatedNoteGameObjects = new List<MusicNoteGameObject>();

        /// <summary>
        /// 同時押し線オブジェクト群
        /// </summary>
        private List<MusicTapLineObject> generatedTapLineObjects = new List<MusicTapLineObject>();

        /// <summary>
        /// タップエフェクトオブジェクト群
        /// </summary>
        private List<MusicTapEffectObject> generatedEffectObjects = new List<MusicTapEffectObject>();

        /// <summary>
        /// 指定されたノーツを画面上に生成する
        /// </summary>
        public void GenerateObject(MusicNote note, int colorIndex = -1)
        {
            // 初期化処理
            MusicNoteGameObject noteObject = GetNoteObject();
            note.SetGameObject(noteObject);

            // 画像設定
            if(note.Place.IsLeftSide() || note.Place.IsRightSide())
            {
                noteObject.NoteImage.sprite = noteSprites[1];
            }
            else
            {
                noteObject.NoteImage.sprite = noteSprites[0];
            }

            // 色設定
            if(colorIndex == -1)
            {
                noteObject.NoteColor = Color.white;
            }
            else
            {
                noteObject.NoteColor = MusicConst.NotesColor[colorIndex];
            }
        }

        /// <summary>
        /// 指定ノーツ同士を繋ぐ同時押し線を生成する
        /// </summary>
        public void GenerateTapLine(MusicNote noteA, MusicNote noteB)
        {
            // 再利用可能オブジェクトがある場合は使用する
            MusicTapLineObject line = generatedTapLineObjects.Find(obj => !obj.IsActive);
            if(line != null)
            {
                GenerateTapLine(line, noteA, noteB);
                return;
            }

            // 生成処理
            line = Instantiate(tapLinePrefab, noteObjectRoot);
            GenerateTapLine(line, noteA, noteB);
            generatedTapLineObjects.Add(line);
        }

        /// <summary>
        /// 指定位置にエフェクトを生成する
        /// </summary>
        public void GenerateEffect(MusicNote effectTargetNote)
        {
            // 再利用可能オブジェクトがある場合は使用する
            MusicTapEffectObject effect = generatedEffectObjects.Find(obj => !obj.IsActive);
            if(effect != null)
            {
                effect.Initialize(MusicTapNotesLocator.Instance.EndPositions[effectTargetNote.Place]);
                return;
            }

            // 生成処理
            effect = Instantiate(effectPrefab, noteObjectRoot);
            effect.Initialize(MusicTapNotesLocator.Instance.EndPositions[effectTargetNote.Place]);
            generatedEffectObjects.Add(effect);
        }

        /// <summary>
        /// 利用可能なノーツを取得する、ない場合は作成する
        /// </summary>
        /// <returns>The note object.</returns>
        private MusicNoteGameObject GetNoteObject()
        {
            // 再利用可能オブジェクトがある場合は使用する
            MusicNoteGameObject noteObject = generatedNoteGameObjects.Find(obj => !obj.IsActive);
            if(noteObject != null)
            {
                return noteObject;
            }

            // 生成処理
            noteObject = Instantiate(noteObjectPrefab, noteObjectRoot) as MusicNoteGameObject;
            generatedNoteGameObjects.Add(noteObject);
            return noteObject;
        }

        /// <summary>
        /// 同時押し線を生成する内部処理
        /// </summary>
        private void GenerateTapLine(MusicTapLineObject line, MusicNote noteA, MusicNote noteB)
        {
            line.Initialize(noteA.NoteObject, noteB.NoteObject);
            noteA.LineObjects.Add(line);
            noteB.LineObjects.Add(line);
        }
    }
}