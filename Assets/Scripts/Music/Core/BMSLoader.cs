using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace MusicDoll
{
    /// <summary>
    /// BMS形式(実際はPMS)で記述された譜面を読み込む
    /// 読み込んだデータはMusicSheetデータとして返す
    /// </summary>
    public class BMSLoader
    {
        /// <summary>
        /// 譜面データ
        /// </summary>
        private static MusicSheet sheet;

        // BMS形式ファイルの配置定数を定義
        private const string OPERATION_BPM = "BPM";     // BPM定義行
        private const int PLACE_BPM = 13;       // BPM(0〜255の整数値)
        private const int PLACE_EX_BPM = 18;    // 拡張BPM(実数値)

        /// <summary>
        /// BMSの配置命令番号を配置レーン種別に変換する
        /// </summary>
        private static readonly Dictionary<int, MusicPlaceKind> placeIntToKind = new Dictionary<int, MusicPlaceKind>
        {
            { 11, MusicPlaceKind.LeftUpper},
            { 12, MusicPlaceKind.LeftLower},
            { 13, MusicPlaceKind.Center1},
            { 14, MusicPlaceKind.Center2},
            { 15, MusicPlaceKind.Center3},
            { 22, MusicPlaceKind.Center4},
            { 23, MusicPlaceKind.Center5},
            { 24, MusicPlaceKind.RightLower},
            { 25, MusicPlaceKind.RightUpper}
        };

        /// <summary>
        /// 譜面を読み込み、譜面データを返す
        /// </summary>
        public static MusicSheet Load(MasterData.MusicMasterData master)
        {
            // 譜面データ初期化
            sheet = new MusicSheet(master.Id);
            sheet.InitializeLoad();

            // 譜面ファイル情報を取得する
            string fileName = string.Format("{0}/sheet3.pms", master.FileName);
            FileInfo fi = new FileInfo(Application.dataPath + "/MusicResources/Resources/" + fileName);
            if(fi == null)
            {
#if DEBUG
                Debug.Log("【エラー】譜面ファイル[" + fileName + "]がありません。");
#endif
                return null;
            }

            // 譜面ファイルを読み込む
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.Default))
            {
                // データを1行ずつ読む
                ReadBMSData(sr);

                // 全データ読み込み完了後の処理
                sheet.InitializePlay();
            }

            return sheet;
        }

        /// <summary>
        /// 実際に1行ずつBMSファイルを読み込んで処理を行う
        /// </summary>
        private static void ReadBMSData(StreamReader sr)
        {
            string readBuffer;

            // 1行ずつ読み込む
            while (sr.Peek() >= 0)
            {
                readBuffer = sr.ReadLine();

                // #から始まらない行は読み込まない
                if (!readBuffer.StartsWith("#", StringComparison.CurrentCulture))
                {
                    continue;
                }

                // 先頭の#を外す
                readBuffer = readBuffer.Substring(1);

                // BPMの行の場合は専用の処理を行う
                if (ReadBPMLine(readBuffer))
                {
                    continue;
                }

                // 通常のノーツを読み込む
                ReadNoteLine(readBuffer);
            }
        }

        /// <summary>
        /// BPMに関する行を処理する
        /// ここではBPMの数値定義を行い、実際のBPM変更は他のノーツと同様に処理する
        /// BPM設定とは異なる行の場合はfalseを返す
        /// </summary>
        private static bool ReadBPMLine(string readBuffer)
        {
            // 先頭が数字の場合はBPM設定ではない
            if (char.IsDigit(readBuffer[0]))
            {
                return false;
            }

            string[] readBufferArray = readBuffer.Split(' ');

            // 初期BPM設定処理
            if (readBufferArray[0].Equals(OPERATION_BPM))
            {
                int bpm = MusicTempoManager.GetBpmScaleValue(readBufferArray[1]);
                sheet.StartBpm = bpm;
            }
            // 拡張BPM設定処理
            else if (readBufferArray[0].StartsWith(OPERATION_BPM, StringComparison.CurrentCulture))
            {
                string bpmKey = readBufferArray[0].Substring(3, 2);
                sheet.AddBPMDefinition(bpmKey, readBufferArray[1]);
            }

            return true;
        }

        /// <summary>
        /// 通常のノーツを配置する行を読み配置する
        /// BPM変更命令等、譜面中にタイミングが指定されるものもここで処理する
        /// </summary>
        private static void ReadNoteLine(string readBuffer)
        {
            //float offset = 1.6f;  // 譜面のずれを修正する時間幅
            int measure;    // 配置する小節
            int command;    // 命令番号（10未満の場合は特殊命令、10以上の場合は配置レーン指定）

            // 配置場所とノーツを分割する
            string[] readBufferArray = readBuffer.Split(':');

            // 配置小節と命令番号を読み込む(例:12304の場合、123小節目の命令番号04)
            measure = int.Parse(readBufferArray[0].Substring(0, 3));
            command = int.Parse(readBufferArray[0].Substring(3, 2));

            // ノーツ配置にかかわらないレーン（BPM変更等）
            if (command < 10)
            {
                // BPM変化
                if (command == PLACE_BPM)
                {
                    ReadLocationLine(readBufferArray[1], (int position, string noteCode) =>
                    {
                        // 16進数表記のため10進数として変換
                        int bpm = int.Parse(noteCode, System.Globalization.NumberStyles.HexNumber);
                        sheet.AddBPMChange(bpm, measure, position);
                    });
                }
                // 拡張表記BPM変化
                else if (command == PLACE_EX_BPM)
                {
                    ReadLocationLine(readBufferArray[1], (int position, string noteCode) =>
                    {
                        // 事前に登録されているBPM定義で登録する
                        sheet.AddBPMChange(noteCode, measure, position);
                    });
                }
            }
            else
            {
                // 実際のノーツ配置
                ReadLocationLine(readBufferArray[1], (int position, string noteCode) =>
                {
                    sheet.AddMusicNote(placeIntToKind[command], measure, position);
                });
            }
        }

        /// <summary>
        /// 実際の配置指定行を読み込み、読み込んだ各ノーツに対しNoteActionを行う
        /// </summary>
        private static void ReadLocationLine(string readBuffer, Action<int, string> NoteAction)
        {
            int noteCount = readBuffer.Length / 2;  // 1ノーツあたり2桁の半角英数字で表現されるため2で割る
            string noteCode;
            for (int i = 0; i < noteCount; i++)
            {
                noteCode = readBuffer.Substring(i * 2, 2);
                if (noteCode != "00")
                {
                    // 配置位置は[最初から数えて何番目か * 1小節あたりの最大分割数 / この小節の分割数]で算出する
                    NoteAction(i * MusicConst.POSITION_FINENESS / noteCount, noteCode);
                }
            }
        }
    }
}