using UnityEngine;
using UnityEngine.UI;

namespace MusicDoll
{
	/// <summary>
	/// タップ時のエフェクト
	/// </summary>
	public class MusicTapEffectObject : MonoBehaviour
	{
        [SerializeField]
        private Image image = null;

        private float timer = 0f;
        private Color color = Color.white;

        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// エフェクトが表示されているか
        /// </summary>
        public bool IsActive { get { return gameObject.activeSelf; } }

        /// <summary>
        /// エフェクト表示時間
        /// </summary>
        private static readonly float EffectTime = 0.5f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, 0f, 180f * timer);
            transform.localScale = Vector3.one * (1f + timer * 2f);
            color.a = 0.5f - timer;
            image.color = color;

            if(timer > EffectTime)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        public void Initialize(Vector2 position)
        {
            timer = 0f;
            rectTransform.anchoredPosition = position;
            gameObject.SetActive(true);
        }
    }
}