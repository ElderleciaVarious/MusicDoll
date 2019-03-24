using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MusicDoll.MasterData
{
	/// <summary>
	/// マスターデータ管理クラスの基底クラス
	/// </summary>
	public abstract class AMasterDataManager<T> where T : AMasterData, new()
	{
        /// <summary>
        /// 全マスターデータ
        /// </summary>
        protected Dictionary<ulong, T> master = new Dictionary<ulong, T>();

        /// <summary>
        /// ファイルからマスターデータを読み込む
        /// </summary>
        public virtual void LoadData(string fileName)
        {
            // マスターデータを初期化する
            master.Clear();

            // ファイル情報を取得する
            FileInfo fi = new FileInfo(Application.dataPath + "/AssetResources/MasterData/" + fileName);
            if(fi == null)
            {
#if DEBUG
                Debug.Log("【エラー】マスターデータ[" + fileName + "]がありません。");
#endif
                return;
            }

            // マスターデータファイルを読み込む
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.Default))
            {
                while(sr.Peek() >= 0)
                {
                    T masterData = new T();
                    masterData.Initialize(sr.ReadLine().Split(','));
                    master.Add(masterData.Id, masterData);
                }
            }
        }

        /// <summary>
        /// IDからマスターを取得する
        /// </summary>
        public T GetData(ulong id)
        {
            if(!master.ContainsKey(id))
            {
                return default(T);
            }

            return master[id];
        }

        /// <summary>
        /// 条件に合うマスターデータを全て取得する
        /// </summary>
        public List<T> GetDataList(System.Func<T, bool> predicate)
        {
            return master.Values.Where(predicate).ToList();
        }
    }
}