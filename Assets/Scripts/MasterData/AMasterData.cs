namespace MusicDoll.MasterData
{
	/// <summary>
	/// マスターデータ抽象クラス
	/// </summary>
	public abstract class AMasterData
	{
        /// <summary>
        /// マスターID
        /// </summary>
        public ulong Id { protected set; get; }

        /// <summary>
        /// 要素名
        /// </summary>
        public string Name { protected set; get; }

        /// <summary>
        /// 読み込んだ文字列配列から初期化する
        /// </summary>
        public virtual void Initialize(string[] data)
        {
            Id = ulong.Parse(data[0]);
            Name = data[1];
        }
	}
}