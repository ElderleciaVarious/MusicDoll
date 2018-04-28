using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicTempoChange{

	// テンポ変化に関わる情報
	private class TempoData {
		private float bpm;
		public float Bpm {
			get { return bpm; }
		}
		private int timing;         // カウント
		public int Timing {
			get { return timing; }
		}
		public float TotalTime { get; set; }    // テンポが変わるまでの実時間

		public TempoData(float bpm, int timing) {
			this.bpm = bpm;
			this.timing = timing;
		}
	}
	private List<TempoData> tempoList;

	public MusicTempoChange() {
		tempoList = new List<TempoData>();
	}

	// テンポ変化を追加
	public void Add(float bpm, int timing) {
		tempoList.Add(new TempoData(bpm, timing));
	}

	// 指定した時間でのカウントを返す
	public int GetTimingFromTime(float time) {

		TempoData tempo = tempoList[0];

		int nowTempoChangeIndex;
		for (nowTempoChangeIndex = 1; nowTempoChangeIndex < tempoList.Count; nowTempoChangeIndex++) {
			tempo = tempoList[nowTempoChangeIndex];
			if (time < tempo.TotalTime) {
				break;
			}
		}

		tempo = tempoList[nowTempoChangeIndex - 1];
		int timing = tempo.Timing;
		timing += (int)((time - tempo.TotalTime) * 2400.0f * (tempo.Bpm / 60.0f));
		return timing;
	}

	// 譜面を読み込み終えたらテンポ変化の実時間を計算する
	public void SetTempoTotalTime() {

		// 最初のBPM
		TempoData tempo = tempoList[0];
		float beforeBpm = tempo.Bpm;
		tempo.TotalTime = 0.0f;

		// 順にテンポ変化が起こる時間を求める
		float totalTime = 0.0f;
		int beforeTiming = 0;
		for (int i = 1; i < tempoList.Count; i++) {
			tempo = tempoList[i];
			totalTime += (tempo.Timing - beforeTiming) / 2400.0f * (60.0f / beforeBpm);
			tempo.TotalTime = totalTime;
			beforeBpm = tempo.Bpm;
			beforeTiming = tempo.Timing;
		}
	}
}
