namespace MusicDoll
{
    /// <summary>
    /// 各譜面の各ノーツ1つ1つを表現するクラス
    /// </summary>
    public class MusicNote
    {
        /// <summary>
        /// 小節番号
        /// </summary>
        private int measure;

        /// <summary>
        /// 小節内での配置
        /// 1小節をPOSITION_FINENESSで分割した中での場所を整数値で格納する
        /// </summary>
        public int LocalPosition { private set; get; }

        /// <summary>
        /// 楽曲全体における位置
        /// 1小節進むあたりPOSITION_FINENESSを加算する
        /// </summary>
        public int GlobalPosition { private set; get; }

        /// <summary>
        /// レーン配置
        /// </summary>
        public MusicPlaceKind Place { private set; get; }

        /// <summary>
        /// 画面内に表示されている場合はtrue
        /// </summary>
        public bool IsActive { get{ return NoteObject != null; } }

        /// <summary>
        /// すでに生成されていたらtrue
        /// </summary>
        public bool IsAppeared { get; set; }

        /// <summary>
        /// 生成済みの表示用GameObject
        /// </summary>
        public MusicNoteGameObject NoteObject { private set; get; }

        public MusicNote(int measure, int position, MusicPlaceKind place)
        {
            this.measure = measure;
            this.LocalPosition = position;
            this.GlobalPosition = MusicConst.POSITION_FINENESS * measure + position;
            this.Place = place;

            IsAppeared = false;
            NoteObject = null;
        }

        /// <summary>
        /// 対応する生成済みGameObjectを設定する
        /// </summary>
        public void SetGameObject(MusicNoteGameObject noteObject)
        {
            NoteObject = noteObject;

            IsAppeared = true;

            noteObject.Initialize(this);
        }

        /// <summary>
        /// このオブジェクトを破棄する
        /// 必ずMusicSheet.DestroyObjectから呼び出される
        /// </summary>
        public void DestroyObject()
        {
            NoteObject.DestroyObject();
            NoteObject = null;
        }

        /// <summary>
        /// 表示位置を動かす
        /// </summary>
        /// <param name="displayCount">1画面内に表示する譜面カウント</param>
        public void Move(int position, int displayCount)
        {
            NoteObject.Move((float)(GlobalPosition - position) / displayCount);
        }
    }
}