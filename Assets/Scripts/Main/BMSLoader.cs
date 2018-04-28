/**
 * 
 * 	BMS形式で記述された譜面を読み込む
 * 	読み込んだ各ノーツはMusicRhythmObjectクラスとして保存される
 * 	読み込んだその他の命令はMusicGameManagerに直接渡す
 * 
**/

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;

public class BMSLoader{

	// 譜面読み込み
	public void FileLoad(string fileName) {
		FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Data/" + fileName);
		try {
			StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.Default);

			string readBuffer;
			string[] readBufferArray;
			char[] splitChar = { '#' };

			float offset = 1.6f;	// 譜面のずれを修正する時間幅
			int measure;	// 配置する小節
			int place;		// 配置するレーン
			int noteNum;    // 1小節内でのノーツ分割数
			string noteCode;
			float[] bpmList = new float[256];

			while (sr.Peek() >= 0) {
				readBuffer = sr.ReadLine();

				// #から始まらない行は読み込まない
				if (!readBuffer.StartsWith("#")) continue;

				// #を外す
				readBufferArray = readBuffer.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
				readBuffer = readBufferArray[0];

				// 先頭が数値ではない行は個別に処理
				if (!char.IsDigit(readBuffer[0])) {
					readBufferArray = readBuffer.Split(' ');
					if (readBufferArray[0] == "BPM") {
						MusicPlayManager.Instance.tempoManager.Add(int.Parse(readBufferArray[1]), 0);
					} else if (readBufferArray[0].StartsWith("BPM")) {  // 拡張BPMの設定
						bpmList[int.Parse(readBufferArray[0].Substring(3, 2))] = float.Parse(readBufferArray[1]);
					}
					continue;
				}

				// 配置場所とノーツを分割する
				readBufferArray = readBuffer.Split(':');

				// 配置小節を読み込む
				measure = int.Parse(readBufferArray[0].Substring(0, 3));

				// 配置レーンを読み込む
				place = ChannelToPlace(readBufferArray[0].Substring(3, 2));

				// ノーツ配置にかかわるレーンではない場合読み込まない
				if (place >= 10) {
					// BPM変化
					if (place == 13) {
						noteNum = readBufferArray[1].Length / 2;
						for (int i = 0; i < noteNum; i++) {
							noteCode = readBufferArray[1].Substring(i * 2, 2);
							if (noteCode != "00") {
								MusicPlayManager.Instance.tempoManager.Add(Int32.Parse(noteCode, System.Globalization.NumberStyles.HexNumber), measure * 9600 + i * 9600 / noteNum);
							}
						}
					} else if (place == 18) {	// 拡張表記BPM変化
						noteNum = readBufferArray[1].Length / 2;
						for (int i = 0; i < noteNum; i++) {
							noteCode = readBufferArray[1].Substring(i * 2, 2);
							if (noteCode != "00") {
								MusicPlayManager.Instance.tempoManager.Add(bpmList[Int32.Parse(noteCode, System.Globalization.NumberStyles.HexNumber)], measure * 9600 + i * 9600 / noteNum);
							}

						}
					}
					continue;
				}

				// 配置ノーツを読み込む
				noteNum = readBufferArray[1].Length / 2;
				for (int i = 0; i < noteNum; i++) {
					noteCode = readBufferArray[1].Substring(i*2, 2);
					if (noteCode != "00") {
						MusicPlayManager.Instance.notesManager.Add(place, noteCode, measure, i, noteNum);
					}
				}
			}

			// テンポ変化に対応
			MusicPlayManager.Instance.tempoManager.SetTempoTotalTime();

			// オフセット
			MusicPlayManager.Instance.MusicOffset = offset;

		} catch (FileNotFoundException e) {
			Debug.LogError("ファイル・ネーヨ" + e.StackTrace);
		}
	}

	// 読み込んだチャンネルをレーン番号に変換、レーン上オブジェクトではない場合は10を加算した値を返す
	private int ChannelToPlace(string channel) {
		int place = int.Parse(channel);

		if (place / 10 == 0) {
			return place + 10;
		} else {
			place %= 10;
			if (place == 6) {
				return 10;
			}else if (place >= 8) {
				place -= 2;
			}
			place--;
		}

		return place;
	}
}
