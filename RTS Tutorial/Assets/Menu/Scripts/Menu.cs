using UnityEngine;
using System.Collections;
using RTS;

public class Menu : MonoBehaviour {
	
	public GUISkin mySkin;
	public Texture2D header;
	
	protected string[] buttons;

	/*** game engine methods ***/

	protected virtual void Start () {
		SetButtons();
	}
	
	protected virtual void OnGUI() {
		DrawMenu();
	}

	/*** protected methods ***/

	/// <summary>
	/// Draws the menu.
	/// </summary>
	protected virtual void DrawMenu() {
		//default implementation for a menu consisting of a vertical list of buttons
		GUI.skin = mySkin;
		float menuHeight = GetMenuHeight();
		
		float groupLeft = Screen.width / 2 - ResourceManager.MenuWidth / 2;
		float groupTop = Screen.height / 2 - menuHeight / 2;
		GUI.BeginGroup(new Rect(groupLeft, groupTop, ResourceManager.MenuWidth, menuHeight));
		
		//background box
		GUI.Box(new Rect(0, 0, ResourceManager.MenuWidth, menuHeight), "");
		//header image
		GUI.DrawTexture(new Rect(ResourceManager.Padding, ResourceManager.Padding, ResourceManager.HeaderWidth, ResourceManager.HeaderHeight), header);
		
		//menu buttons
		if(buttons != null) {
			float leftPos = ResourceManager.MenuWidth / 2 - ResourceManager.ButtonWidth / 2;
			float topPos = 2 * ResourceManager.Padding + header.height;
			for(int i = 0; i < buttons.Length; i++) {                if(i > 0) topPos += ResourceManager.ButtonHeight + ResourceManager.Padding;
				if(GUI.Button(new Rect(leftPos, topPos, ResourceManager.ButtonWidth, ResourceManager.ButtonHeight), buttons[i])) {
					HandleButton(buttons[i]);
				}
			}
		}
		
		GUI.EndGroup();
	}

	/// <summary>
	/// Sets the buttons.
	/// </summary>
	protected virtual void SetButtons() {
		//a child class needs to set this for buttons to appear
	}

	/// <summary>
	/// Handles the button.
	/// </summary>
	/// <param name="text">Text.</param>
	protected virtual void HandleButton(string text) {
		//a child class needs to set this to handle button clicks
	}

	/// <summary>
	/// Gets the height of the menu.
	/// </summary>
	/// <returns>The menu height.</returns>
	protected virtual float GetMenuHeight() {
		float buttonHeight = 0;
		if(buttons != null) buttonHeight = buttons.Length * ResourceManager.ButtonHeight;
		float paddingHeight = 2 * ResourceManager.Padding;
		if(buttons != null) paddingHeight += buttons.Length * ResourceManager.Padding;
		return ResourceManager.HeaderHeight + buttonHeight + paddingHeight;
	}

	/// <summary>
	/// Exits the game.
	/// </summary>
	protected void ExitGame() {
		Application.Quit();
	}
}