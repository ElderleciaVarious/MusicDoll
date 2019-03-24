using System.Collections;
using System.Collections.Generic;
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
        private GameObject startObject;
        /// <summary>
        /// 同時押し線の終端を指すオブジェクト
        /// </summary>
        private GameObject endObject;

        /// <summary>
        /// 線の描画
        /// </summary>
        private LineRenderer line;

        private void Update()
        {
            if(startObject == null || endObject == null)
            {
                Destroy(gameObject);
                return;
            }

            line.SetPosition(0, startObject.transform.position);
            line.SetPosition(1, endObject.transform.position);
        }

        /// <summary>
        /// 同時押し線の初期設定
        /// </summary>
        public void Initialize(GameObject startObject, GameObject endObject)
        {
            this.startObject = startObject;
            this.endObject = endObject;

            line = GetComponent<LineRenderer>();
        }
    }
}