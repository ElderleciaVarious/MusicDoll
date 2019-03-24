using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicDoll
{
	/// <summary>
	/// 判定文字とコンボ数表示オブジェクト
	/// </summary>
	public class MusicJudgeObject : MonoBehaviour
	{
        /// <summary>
        /// 判定文字
        /// </summary>
        [SerializeField]
        private Image image = null;

        /// <summary>
        /// コンボ数
        /// </summary>
        [SerializeField]
        private Text combo = null;

        /// <summary>
        /// 表示時間
        /// </summary>
        private float activeTimer;

        /// <summary>
        /// 透明度を指定するための色
        /// </summary>
        private Color activeColor = Color.white;

        private void Update()
        {
            if(activeTimer > 0f)
            {
                activeTimer -= Time.deltaTime;

                activeColor.a = (activeTimer > 0.4f) ? 1f : activeTimer * 2.5f;
                image.color = activeColor;
                combo.color = activeColor;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 判定表示を設定する
        /// </summary>
        public void Set(MusicJudgeKind kind, int comboCount)
        {
            gameObject.SetActive(true);
            combo.text = (comboCount > 0) ? comboCount.ToString() : "";
            activeTimer = 0.6f;
        }
    }
}