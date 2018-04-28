using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MusicPlayManager : SingletonMonoBehaviour<MusicPlayManager> {

	public GameObject noteObj, holdObj, leftObj, rightObj, lineObj, holdLineObj, frickLineObj, tapEffect, frickEffect;

	public GameObject comboCounter, comboText;

	public float hiSpeed = 3.0f;

	private bool isPlaying;
	private float startTime;    // 譜面開始時間
	private bool isMusicPlaying;    // 楽曲を再生されたか
	public float MusicOffset {  // 楽曲を再生するタイミングをずらす幅
		set; get;
	}

	private int combo;

	public MusicNotesManager notesManager;
	public MusicTempoChange tempoManager;

	private Dictionary<MusicNote, GameObject> noteObjectList;
	private List<MusicNote> removeNoteList; // 消去するノーツをここに保持し後から消去
	private GameObject[] holdObjects;   // ロングノーツの始点をここで記憶する
	private GameObject[] frickObjects;   // フリックノーツの始点をここで記憶する

	void Awake() {
		
	}

	void Start () {
		notesManager = new MusicNotesManager();
		tempoManager = new MusicTempoChange();
		noteObjectList = new Dictionary<MusicNote, GameObject>();
		removeNoteList = new List<MusicNote>();
		holdObjects = new GameObject[5];
		frickObjects = new GameObject[2];

		new TextLoader().FileLoad("re2/fumen.bms");
		Play();
	}
	
	void Update () {
		MusicPlayUpdate();
	}

	public void Play() {
		startTime = Time.time + 2.0f;
		isPlaying = true;
	}

	public void Stop() {
		isPlaying = false;
	}

	// 初期化
	private void Init() {
		
	}

	// 譜面を流す処理
	private void MusicPlayUpdate() {

		// 楽曲再生処理
		if (!isMusicPlaying && Time.time - startTime > MusicOffset) {
			GetComponent<AudioSource>().Play();
			isMusicPlaying = true;
		}

		int nowTiming = tempoManager.GetTimingFromTime(Time.time - startTime);

		// ノーツを動かす
		float timingOffset;
		GameObject noteObject;
		foreach (MusicNote note in noteObjectList.Keys) {
			noteObject = noteObjectList[note];

			timingOffset = (note.TotalTiming - nowTiming) / 2000.0f * hiSpeed;
			noteObject.SendMessage("SetPosition", timingOffset);

			if (timingOffset < -0.25f) {
				removeNoteList.Add(note);
			}
		}

		// 過ぎ去ったノーツを消去
		for (int i = 0; i < removeNoteList.Count; i++) {
			if (removeNoteList[i].FrickLineGroup == -1) {	// フリックかどうか
				Instantiate(tapEffect, noteObjectList[removeNoteList[i]].transform.position, Quaternion.identity);
			} else {
				Instantiate(frickEffect, noteObjectList[removeNoteList[i]].transform.position, Quaternion.identity);
			}
			noteObjectList[removeNoteList[i]].SendMessage("Destroy");
			noteObjectList.Remove(removeNoteList[i]);
			combo++;
			comboCounter.GetComponent<Text>().text = combo.ToString();
			comboText.GetComponent<Text>().text = "COMBO";
		}
		removeNoteList.Clear();

		// 新しいノーツの出現を監視
		List<MusicNote> displayNotesList = null;
		GameObject beforeNote = null;
		int frickLineGroup;
		for (int i = 0; i < 5; i++) {
			displayNotesList = notesManager.GetNoteList(i);

			if (displayNotesList.Count > 0) {
				if (displayNotesList[0].TotalTiming < nowTiming + 20000 / hiSpeed) {

					GameObject newNote = null;
					frickLineGroup = displayNotesList[0].FrickLineGroup;    // フリックかどうかを判定するために最初に算出

					// ノーツを生成
					if (holdObjects[i] == null || frickLineGroup != -1) { // 同時押し終端ではない、もしくはフリックの場合
						newNote = Instantiate(GetNotePrefab(displayNotesList[0].Type), Vector3.up * 10f, Quaternion.identity) as GameObject;
					} else {    // フリック以外の同時押し終端の場合はロングノーツの見た目にする
						newNote = Instantiate(holdObj, Vector3.up * 10f, Quaternion.identity) as GameObject;
					}
					newNote.SendMessage("Init", i);

					// 同時押し用判定
					if (beforeNote != null) {
						GameObject line = Instantiate(lineObj, Vector3.zero, Quaternion.identity) as GameObject;
						line.GetComponent<TapLine>().Init(beforeNote, newNote);
					}
					beforeNote = newNote;

					// プレハブによりノーツオブジェクトを表示
					newNote.SendMessage("Init", i);
					noteObjectList.Add(displayNotesList[0], newNote);

					// ロングノーツ用判定
					if (holdObjects[i] != null) {   // 終点
						holdObjects[i].GetComponent<HoldLine>().AddEndObject(newNote);
						holdObjects[i] = null;
					}else if (displayNotesList[0].Type == MusicNote.NoteType.HOLD) {	 // 始点
						GameObject line = Instantiate(holdLineObj, Vector3.zero, Quaternion.identity) as GameObject;
						line.GetComponent<HoldLine>().Init(i, newNote);
						holdObjects[i] = line;	// 始点を記憶
					}

					// フリック判定
					if (displayNotesList[0].FrickLineGroup != -1) {
						if (frickObjects[frickLineGroup] != null) {   // 終点
							frickObjects[frickLineGroup].GetComponent<FrickLine>().AddEndObject(newNote);
							frickObjects[frickLineGroup] = null;
						}
						
						// フリック終端ではなければ補助線を出す
						if (!displayNotesList[0].IsFrickEnd) {
							GameObject line = Instantiate(frickLineObj, Vector3.zero, Quaternion.identity) as GameObject;
							line.GetComponent<FrickLine>().Init(i, newNote);
							frickObjects[frickLineGroup] = line;  // 始点を記憶
						}
					}

					// 表示したので譜面情報リストからは消去する
					notesManager.RemoveTopNote(i);
				}
			}
		}
	}

	private GameObject GetNotePrefab(MusicNote.NoteType type) {
		switch (type) {
			case MusicNote.NoteType.NORMAL:
				return noteObj;
			case MusicNote.NoteType.HOLD:
				return holdObj;
			case MusicNote.NoteType.LEFTFRICK:
				return leftObj;
			case MusicNote.NoteType.RIGHTFRICK:
				return rightObj;
		}

		return null;
	}
}
