using UnityEngine;
using System.Collections.Generic;

namespace FNAAS {
	public enum AudioType {
		Type0,
		Type1,
		Type2,
		Type3,
		Type4,
		Type5
	}

	public class AmbientAudioController : BaseController {

		// --------------- Tweakables ---------------

		private int maxActiveSpeakers = 16;
		private float audioRange = 90f;

		// --------------- Tweakables ---------------

		//private bool speakersHaveChanged;

		private Transform playerTransform;

		private List<Speaker> currentActiveSpeakers;
		private List<Speaker> inRangeSpeakers;
		private List<Speaker> playableSpeakers;

		private Speaker[][] speakerGroups;
		private List<Node> audioTypeNodes;
		private Dictionary<Speaker, Vector3> allSpeakers; // List of all speakers created in this session -> for saving

		public override void Initialize() {
			base.Initialize();

			allSpeakers = new Dictionary<Speaker, Vector3>();
			inRangeSpeakers = new List<Speaker>();
			currentActiveSpeakers = new List<Speaker>();

			audioTypeNodes = SetDefaultWeights();

			playerTransform = GameAccesPoint.Instance.mainGameState._playerController._playerTransform;
			GameAccesPoint.Instance.mainGameState._worldController.onChunkSpawned += OnChunkChanged;

			UpdateInRangeSpeakers();

			isInit = true;
		}

		public void AddSpeaker(Speaker speaker, Vector3 position) {
			allSpeakers.Add(speaker, position);
			speaker.SetPattern();
		}

		float currentTime = 0;

		public override void UpdateGameTime(float globalGameTime, float deltaGameTime) {
			if (!isInit)
				return;

			currentTime += Time.deltaTime;

			if (currentTime > 4f) {
				PlayNext();
				currentTime = 0;
			}
		}

		private void UpdateInRangeSpeakers() {
			inRangeSpeakers.Clear();

			foreach (KeyValuePair<Speaker, Vector3> pair in allSpeakers) {
				if (Vector3.Distance(pair.Value, playerTransform.position) <= audioRange) {
					inRangeSpeakers.Add(pair.Key);
				}
			}

			//speakersHaveChanged = true;
        }

		private void PlayNext() {
			GetNextPlayAudio();

			if (playableSpeakers != null) {
				foreach (Speaker s in playableSpeakers) {
					s.PlayNext();
				}
			}
		}

		private void GetNextPlayAudio() {
			currentActiveSpeakers.Clear();

			List<Speaker> inRangeAtMoment = inRangeSpeakers;
			foreach (Speaker s in inRangeAtMoment) {
				if (s.WantsPlayNext())
					currentActiveSpeakers.Add(s);
			}

			if (currentActiveSpeakers.Count <= 0) {
				return; // Do not update further
			}

			playableSpeakers = CheckAudioGroups();
		}

		private void UpdateSpeakerGroups() { // TODO: Check efficiency
			speakerGroups = new Speaker[4][];

			List<Speaker> frontLeft = new List<Speaker>();
			List<Speaker> frontRight = new List<Speaker>();
			List<Speaker> backLeft = new List<Speaker>();
			List<Speaker> backRight = new List<Speaker>();

			//Debug.Log("Currentactivespeakers: " + currentActiveSpeakers.Count);

			foreach (Speaker s in currentActiveSpeakers) {
				Vector3 heading = allSpeakers[s] - playerTransform.position;
				float dotForward = Vector3.Dot(heading, playerTransform.forward);
				float dotRight = Vector3.Dot(heading, playerTransform.right);

				if (dotForward > 0) {
					if (dotRight > 0) {
						frontRight.Add(s);
						//Debug.Log("In front right: " + s, s.gameObject);
					} else {
						frontLeft.Add(s);
						//Debug.Log("In front left: " + s, s.gameObject);
					}
				} else {
					if (dotRight > 0) {
						backRight.Add(s);
						//Debug.Log("In back right: " + s, s.gameObject);
					} else {
						backLeft.Add(s);
						//Debug.Log("In back left: " + s, s.gameObject);
					}
				}
			}

			speakerGroups[0] = frontLeft.ToArray();
			speakerGroups[1] = frontRight.ToArray();
			speakerGroups[2] = backLeft.ToArray();
			speakerGroups[3] = backRight.ToArray();

			//for (int i = 0; i < speakerGroups.Length; i++) {
			//	Debug.Log("Lenght: " + speakerGroups[i].Length + " | Index: " + i);
			//}
		}

		private List<Speaker> CheckAudioGroups() {
			UpdateSpeakerGroups();
			audioTypeNodes = SetDefaultWeights();

			List<Speaker> tmpPlayableSpeakers = new List<Speaker>();

			//for (int i = 0; i < speakerGroups.Length; i++) {
			//	for (int j = 0; j < speakerGroups[i].Length; j++) {
			//		Speaker speaker = CheckSpeaker(speakerGroups[i][j]);
			//		if (speaker != null) {
			//			playableSpeakers.Add(speaker);
			//		}
			//	}
			//}

			int groupIndex = 0;
			int itterationCount = 0;
			int totalSpeakers = 0;

			foreach (Speaker[] s in speakerGroups) {
				totalSpeakers += s.Length;
			}

			//Debug.Log("Total speaker count: " + totalSpeakers);

			while (true) {
				int randomIndex = Random.Range(0, speakerGroups[groupIndex].Length - 1);
				if (speakerGroups[groupIndex].Length > 0 && randomIndex < speakerGroups[groupIndex].Length) {
					Speaker speaker = speakerGroups[groupIndex][randomIndex];
					Speaker returnedSpeaker = CheckSpeaker(speaker);
					if (returnedSpeaker != null) {
						if (!tmpPlayableSpeakers.Contains(returnedSpeaker) && returnedSpeaker.isActiveAndEnabled)
							tmpPlayableSpeakers.Add(returnedSpeaker);
					}
				}

				//Debug.Log("gIndex: " + groupIndex + "itt: " + itterationCount);

				groupIndex++;
				if (groupIndex > speakerGroups.Length - 1) {
					groupIndex = 0;
				}
				itterationCount++;
				if (itterationCount > totalSpeakers - 1 || tmpPlayableSpeakers.Count >= maxActiveSpeakers) {
					return tmpPlayableSpeakers;
				}
			}

			//int count = 0;
			//foreach (Speaker s in playableSpeakers) {
			//	Debug.Log("Speaker: " + s + " | Idx: " + count, s.gameObject);
			//	count++;
			//}

			//Debug.Log("Playable: " + playableSpeakers.Count);
		}

		private Speaker CheckSpeaker(Speaker speaker) {
			float average = GetAverageWeight();

			foreach (Node n in audioTypeNodes) {
				if (n.type == speaker.audioType) {
					if (n.weight <= average) {
						n.weight++;
						return speaker;
					}
				}
			}

			return null;
		}

		private float GetAverageWeight() {
			int total = 0;

			foreach (Node n in audioTypeNodes) {
				total += n.weight;
			}

			return total / audioTypeNodes.Count;
		}

		private List<Node> SetDefaultWeights() {
			List<Node> tmpNodeList = new List<Node>();

			System.Array values = System.Enum.GetValues(typeof(AudioType));

			foreach (AudioType type in values) {
				tmpNodeList.Add(new Node(type, 0));
			}

			return tmpNodeList;
		}

		private void OnChunkChanged(Chunk chunk) {
			UpdateInRangeSpeakers();
		}

	}
}

public class Node {
	public FNAAS.AudioType type;
	public int weight;

	public Node(FNAAS.AudioType type, int weight) {
		this.type = type;
		this.weight = weight;
	}
}