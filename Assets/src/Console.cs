using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

class Console : MonoBehaviour {
	public string consoleDropDownKey = "`";
	public Texture consoleBackground;
	public float heightPercent = 0.25f;
	public float slideSpeed = 5f;
	public float fontSizeInPixels = 20f;
	
	bool consoleDown;
	bool inTransit;
	
	string currentCommand = "";
	Stack<string> output = new Stack<string>();
	
	static Console instance;
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="Console"/> listens for console drop down.
	/// </summary>
	/// <value>
	/// Use <c>false</c> to disable console listening (ex: on other UI focus).
	/// </value>
	public static bool ListenForConsoleDropDown { get; set; }
	
	/// <summary>
	/// Gets a value indicating whether the console is open.
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance is open; otherwise, <c>false</c>.
	/// </value>
	public static bool IsOpen {
		get { return instance.consoleDown; }
	}
	
	void Start() {
		instance = this;
		ListenForConsoleDropDown = true;
	}

	void Update() {
		if (ListenForConsoleDropDown && Input.GetKeyUp(consoleDropDownKey)) {
			ToggleConsole();
		}
		if (consoleDown) {
			foreach( var character in Input.inputString) {
				switch(character) {
				case '\b':
					if (currentCommand == "") break;
					currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
					break;
				case '\n':
					output.Push(currentCommand);
					// TODO: Execute command
					ExecuteCommand(currentCommand);
					currentCommand = "";
					break;
				default:
					if (character == consoleDropDownKey[0]) break; // we don't want our drop-down key to type, as it has special meaning!
					currentCommand += character;
					break;
				}
			}
		}
		
		if (inTransit) {
//			SlideConsole(consoleDown);
		}
	}
	
	void ToggleConsole() {
		consoleDown = !consoleDown;
//		inTransit = true;
		
	}
	
	void OnGUI() {
		if (consoleDown) {
			DrawBackground();
			DrawCurrentCommand();
			DrawOutput();
		}
	}
	
	void DrawBackground() {
		GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height / 2f), consoleBackground);
	}
	
	void DrawCurrentCommand() {
		// Maybe after I get more time to read the GUI API
		//var height = GUI.skin.GetStyle("Label").CalcHeight();
		var commandContent = new GUIContent("> " + currentCommand);
		GUI.Label(new Rect(0f, Screen.height / 2f - fontSizeInPixels, Screen.width, fontSizeInPixels), commandContent);
	}
	
	void DrawOutput() {
		var i = 2;
		foreach (var outputLine in output) {
			var outputContent = new GUIContent(outputLine);
			GUI.Label(new Rect(0f, Screen.height / 2f - (fontSizeInPixels * i), Screen.width, fontSizeInPixels), outputContent);
			++i;
		}
	}
	
	void ExecuteCommand(string commandText) {
		var words = commandText.Split(' ');
		var commandName = words.First();
		var arguments = string.Join(" ", words);
		
		InvokeCommand(commandName, arguments);
	}
	
	void InvokeCommand(string commandName, string arguments) {
		var gameObjects = FindSceneObjectsOfType(typeof(GameObject)).Cast<GameObject>();
		foreach(var sceneGameObject in gameObjects) {
			var components = sceneGameObject.GetComponents<MonoBehaviour>();
			foreach (var component in components) {
				var methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
				
				foreach (var method in methods) {
					var attributes = method.GetCustomAttributes(typeof(ConsoleCommandAttribute), false).Cast<ConsoleCommandAttribute>();
					if (attributes.Count() == 0) continue;
					
					if (commandName.ToLower() == attributes.First().CommandName.ToLower()) {
						method.Invoke(component, new [] {arguments});
					}
				}
			}
		}
	}
	
//	void SlideConsole(bool moveDown) {
//		
//	}
}
