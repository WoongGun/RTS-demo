using UnityEngine;
using RTS;
using UnityEngine;
using RTS;

public class PauseMenu : Menu {
	
	private Player player;

	/*** game engine methods ***/

	protected override void Start () {
		base.Start();
		player = transform.root.GetComponent< Player >();
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) Resume();
	}

	/*** protected methods ***/

	/// <summary>
	/// Sets the buttons.
	/// </summary>
	protected override void SetButtons () {
		buttons = new string[] {"Resume", "Exit Game"};
	}

	/// <summary>
	/// Handles the button.
	/// </summary>
	/// <param name="text">Text.</param>
	protected override void HandleButton (string text) {
		switch(text) {
		case "Resume": Resume(); break;
		case "Exit Game": ReturnToMainMenu(); break;
		default: break;
		}
	}

	/*** private methods ***/

	private void ReturnToMainMenu() {
		Application.LoadLevel("MainMenu");
		Screen.showCursor = true;
	}

	private void Resume() {
		Time.timeScale = 1.0f;
		GetComponent< PauseMenu >().enabled = false;
		if(player) player.GetComponent< UserInput >().enabled = true;
		Screen.showCursor = false;
		ResourceManager.MenuOpen = false;
	}
	
}