using UnityEngine;
using System.Collections;
using RTS;

public class MainMenu : Menu {

	/*** protected methods ***/

	/// <summary>
	/// Sets the buttons.
	/// </summary>
	protected override void SetButtons () {
		buttons = new string[] {"New Game", "Quit Game"};
	}

	/// <summary>
	/// Handles the button.
	/// </summary>
	/// <param name="text">Text.</param>
	protected override void HandleButton (string text) {
		switch(text) {
		case "New Game": NewGame(); break;
		case "Quit Game": ExitGame(); break;
		default: break;
		}
	}

	/*** private methods ***/

	private void NewGame() {
		ResourceManager.MenuOpen = false;
		Application.LoadLevel("Map");
		//makes sure that the loaded level runs at normal speed
		Time.timeScale = 1.0f;
	}
}