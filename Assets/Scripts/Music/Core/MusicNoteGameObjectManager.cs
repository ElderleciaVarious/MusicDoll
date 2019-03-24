using UnityEngine;
using UnityEngine.UI;

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
        private GameObject noteObjectPrefab = null;

        /// <summary>
        /// ノーツ生成ルート
        /// </summary>
        [SerializeField]
        private Transform noteObjectRoot = null;

        [SerializeField]
        private Sprite[] noteSprites = null;

        /// <summary>
        /// 指定されたノーツを画面上に生成する
        /// </summary>
        public void GenerateObject(MusicNote note)
        {
            // 生成処理
            GameObject obj = Instantiate(noteObjectPrefab) as GameObject;
            obj.transform.SetParent(noteObjectRoot);
            MusicNoteGameObject noteObject = obj.GetComponent<MusicNoteGameObject>();

            // 初期化処理
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
        }
    }
}