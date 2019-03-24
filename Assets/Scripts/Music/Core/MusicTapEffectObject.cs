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

        private static readonly float EffectTime = 0.5f;

        private void Update()
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, 0f, 180f * timer);
            transform.localScale = Vector3.one * (1f + timer * 2f);
            color.a = 0.5f - timer;
            image.color = color;

            if(timer > EffectTime)
            {
                Destroy(gameObject);
            }
        }
    }
}