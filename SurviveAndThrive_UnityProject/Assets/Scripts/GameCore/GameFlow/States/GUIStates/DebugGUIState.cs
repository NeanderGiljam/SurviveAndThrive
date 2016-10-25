using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class DebugGUIState : GuiState {

	private GameObject debugScreen;

	private InputField inputField;
	private Transform contentContainer;
	private GameObject textField;

	private CommandList commandList;

	public override void Initialize() {
		base.Initialize();

		GameAccesPoint.Instance.debugGUIState = this;
		GameAccesPoint.Instance.managerSystem.stateManager.guiStates.Add(this);

		// --------------- Root Setup ---------------
		debugScreen = gameObject;
		debugScreen.transform.SetParent(uiRoot);
		debugScreen.transform.position = uiRoot.position;
		// --------------- Root Setup ---------------

		GameObject inputObject = GameObject.Find("r_DebugInput");
		if (inputObject != null) {
			inputField = inputObject.GetComponent<InputField>();
			if (inputField == null) {
				Debug.LogError("Could not find the input field component", gameObject);
				return;
			}
		} else {
			Debug.LogError("Could not find the input field object", gameObject);
			return;
		}
		
		inputField.ActivateInputField();
		contentContainer = GameObject.Find("r_Content").transform;
		textField = GameObject.Find("r_Text_Element");

		if (textField == null) {
			Debug.LogError("Could not find the text element template", gameObject);
			return;
		}

		if (contentContainer == null) {
			Debug.LogError("Could not find the content container of the scroll rect", gameObject);
			return;
		}


		commandList = new CommandList(this);

		isInit = true;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			CheckInput(inputField.text);
			inputField.text = "";
			inputField.ActivateInputField();
		}
	}

	private void CheckInput(string text) {
		foreach (KeyValuePair<string, UnityAction<object[]>> pair in commandList.commands) {
			if (text.Contains(pair.Key)) {
				//string strippedCommand = text.Replace(pair.Key, "");
				pair.Value.Invoke(CheckCommandForArgs(text));
				return;
			}
		}

		CreateTextField("Error: Invalid Command", Color.red);
	}

	private object[] CheckCommandForArgs(string command) {
		object[] args = new object[] { null };

		//if (command.Contains("Vector3(")) {
		//	string[] result = command.Split(new char[] { '(', ')' });
		//	result[1].Replace(" ", "");
		//	result = result[1].Split(',');
		//	Vector3 newPos = new Vector3(float.Parse(result[0]), float.Parse(result[1]), float.Parse(result[2]));

		//	args[0] = newPos;
		//}

		return args;
	}

	public void CreateTextField(string text, Color textColor) {
		GameObject newTextField = Instantiate(textField);
		Text textElement = newTextField.GetComponent<Text>();

		newTextField.transform.SetParent(contentContainer);
		newTextField.transform.localScale = Vector3.one;

		textElement.text = text;
		textElement.color = textColor;
	}

	public override void SetActiveState(bool state) {
		base.SetActiveState(state);

		debugScreen.SetActive(state);
		inputField.ActivateInputField();
	}
}

public class CommandList {
	public Dictionary<string, UnityAction<object[]>> commands { get; private set; }

	private DebugGUIState debugTerminal;

	public CommandList(DebugGUIState parent) {
		debugTerminal = parent;

		commands = new Dictionary<string, UnityAction<object[]>>() {
			{ "/help", (object[] args) => Help() },
			{ "/current pos", (object[] args) => DebugPos() },
			{ "/spawn", (object[] args) => SpawnObject() },
			{ "/unpause", (object[] args) => UnPause() },
			{ "/teleport", (object[] args) => Teleport(args) }
		};
	}

	private void Help() {
		foreach (KeyValuePair<string, UnityAction<object[]>> pair in commands) {
			debugTerminal.CreateTextField(pair.Key, Color.green);
		}
	}

	private void UnPause() {
		GameAccesPoint.Instance.managerSystem.Resume();
	}

	private void DebugPos() {
		Vector3 currentPos = GameAccesPoint.Instance.mainGameState._playerController._playerTransform.position;
		debugTerminal.CreateTextField("Current player position = " + currentPos, Color.black);
	}

	private void SpawnObject() {
		throw new System.NotImplementedException();
	}

	private void Teleport(object[] args) {
		//GameAccesPoint.Instance.mainGameState._playerController._playerTransform.position = (Vector3)args[0];
		//debugTerminal.CreateTextField("Teleported player to = " + (Vector3)args[0], Color.black);
		throw new System.NotImplementedException();
	} 
}