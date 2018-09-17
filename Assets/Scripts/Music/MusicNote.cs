namespace MusicDoll
{
    /// <summary>
    /// 各譜面の各ノーツ1つ1つを表現するクラス
    /// </summary>
    public class MusicNote
    {
        /// <summary>
        /// 1小節を分割する細かさ
        /// 128×3×5=1920のため、128分や48分が表現可能
        /// </summary>
        public static readonly int POSITION_FINENESS = 1920;

        /// <summary>
        /// 小節番号
        /// </summary>
        private int measure;

        /// <summary>
        /// 小節内での配置
        /// 1小節をPOSITION_FINENESSで分割した中での場所を整数値で格納する
        /// </summary>
        private int position;

        /// <summary>
        /// レーン番号　左サイド上部から右サイド上部まで左から順に1から9
        /// </summary>
        private int place;

        /// <summary>
        /// 画面内に表示されている場合はtrue
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// すでに生成されていたらtrue
        /// </summary>
        public bool IsAppeared { get; set; }

        public MusicNote(int measure, int position, int place)
        {
            this.measure = measure;
            this.position = position;
            this.place = place;

            IsActive = false;
            IsAppeared = false;
        }

        /// <summary>
        /// 楽曲全体における位置を返す
        /// 1小節進むあたりPOSITION_FINENESSを加算する
        /// </summary>
        /// <returns>楽曲全体における位置</returns>
        public long GetTotalPosition()
        {
            return measure * POSITION_FINENESS + position;
        }
    }
}