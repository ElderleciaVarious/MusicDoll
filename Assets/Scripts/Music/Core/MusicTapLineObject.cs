using UnityEngine;

namespace MusicDoll
{
	/// <summary>
	/// 同時押し線オブジェクト
	/// </summary>
	public class MusicTapLineObject : MonoBehaviour
	{
        /// <summary>
        /// 同時押し線の先端を指すオブジェクト
        /// </summary>
        private MusicNoteGameObject startObject;
        /// <summary>
        /// 同時押し線の終端を指すオブジェクト
        /// </summary>
        private MusicNoteGameObject endObject;

        /// <summary>
        /// 線の描画
        /// </summary>
        private LineRenderer line;

        /// <summary>
        /// 関連づいていないノーツか
        /// </summary>
        public bool IsActive { get { return gameObject.activeSelf; } }

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            // どちらかのノーツが消えたら消去
            if (startObject.IsNotePassed || endObject.IsNotePassed)
            {
                Delete();
                return;
            }

            line.SetPosition(0, startObject.transform.position);
            line.SetPosition(1, endObject.transform.position);
        }

        /// <summary>
        /// 同時押し線の初期設定
        /// </summary>
        public void Initialize(MusicNoteGameObject startObject, MusicNoteGameObject endObject)
        {
            this.startObject = startObject;
            this.endObject = endObject;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 同時押し線を消す
        /// </summary>
        public void Delete()
        {
            gameObject.SetActive(false);
        }
    }
}