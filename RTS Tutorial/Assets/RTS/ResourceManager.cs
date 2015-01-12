using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTS{
	public class ResourceManager{

		public static int BuildSpeed { get { return 2; } }
		
		public static int ScrollWidth {get{return 15;}}
		public static float ScrollSpeed {get{return 25;}}
		public static float RotateAmount {get{return 10;}}
		public static float RotateSpeed {get{return 100;}}

		public static float MinCameraHeight {get{return 10;}}
		public static float MaxCameraHeight {get{return 40;}}

		private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
		public static Vector3 InvalidPosition { get { return invalidPosition; } }

		private static GUISkin selectBoxSkin;
		public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }

		private static Texture2D healthyTexture, damagedTexture, criticalTexture;
		public static Texture2D HealthyTexture { get { return healthyTexture; } }
		public static Texture2D DamagedTexture { get { return damagedTexture; } }
		public static Texture2D CriticalTexture { get { return criticalTexture; } }

		public static bool MenuOpen { get; set; }

		private static float buttonHeight = 40;
		private static float headerHeight = 32, headerWidth = 256;
		private static float textHeight = 25, padding = 10;
		public static float PauseMenuHeight { get { return headerHeight + 2 * buttonHeight + 4 * padding; } }
		public static float MenuWidth { get { return headerWidth + 2 * padding; } }
		public static float ButtonHeight { get { return buttonHeight; } }
		public static float ButtonWidth { get { return (MenuWidth - 3 * padding) / 2; } }
		public static float HeaderHeight { get { return headerHeight; } }
		public static float HeaderWidth { get { return headerWidth; } }
		public static float TextHeight { get { return textHeight; } }
		public static float Padding { get { return padding; } }

		private static Dictionary< ResourceType, Texture2D > resourceHealthBarTextures;

		private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
		public static Bounds InvalidBounds { get { return invalidBounds; } }

		private static GameObjectList gameObjectList;

		/*** public methods ***/

		/// <summary>
		/// Stores the select box items.
		/// </summary>
		/// <param name="skin">Skin.</param>
		/// <param name="healthy">Healthy.</param>
		/// <param name="damaged">Damaged.</param>
		/// <param name="critical">Critical.</param>
		public static void StoreSelectBoxItems(GUISkin skin, Texture2D healthy, Texture2D damaged, Texture2D critical) {
			selectBoxSkin = skin;
			healthyTexture = healthy;
			damagedTexture = damaged;
			criticalTexture = critical;
		}

		/// <summary>
		/// Sets the game object list.
		/// </summary>
		/// <param name="objectList">Object list.</param>
		public static void SetGameObjectList(GameObjectList objectList) {
			gameObjectList = objectList;
		}

		/// <summary>
		/// Gets the building.
		/// </summary>
		/// <returns>The building.</returns>
		/// <param name="name">Name.</param>
		public static GameObject GetBuilding(string name) {
			return gameObjectList.GetBuilding(name);
		}

		/// <summary>
		/// Gets the unit.
		/// </summary>
		/// <returns>The unit.</returns>
		/// <param name="name">Name.</param>
		public static GameObject GetUnit(string name) {
			return gameObjectList.GetUnit(name);
		}

		/// <summary>
		/// Gets the world object.
		/// </summary>
		/// <returns>The world object.</returns>
		/// <param name="name">Name.</param>
		public static GameObject GetWorldObject(string name) {
			return gameObjectList.GetWorldObject(name);
		}

		/// <summary>
		/// Gets the player object.
		/// </summary>
		/// <returns>The player object.</returns>
		public static GameObject GetPlayerObject() {
			return gameObjectList.GetPlayerObject();
		}

		/// <summary>
		/// Gets the build image.
		/// </summary>
		/// <returns>The build image.</returns>
		/// <param name="name">Name.</param>
		public static Texture2D GetBuildImage(string name) {
			return gameObjectList.GetBuildImage(name);
		}

		/// <summary>
		/// Gets the resource health bar.
		/// </summary>
		/// <returns>The resource health bar.</returns>
		/// <param name="resourceType">Resource type.</param>
		public static Texture2D GetResourceHealthBar(ResourceType resourceType) {
			if(resourceHealthBarTextures != null && resourceHealthBarTextures.ContainsKey(resourceType)) return resourceHealthBarTextures[resourceType];
			return null;
		}

		/// <summary>
		/// Sets the resource health bar textures.
		/// </summary>
		/// <param name="images">Images.</param>
		public static void SetResourceHealthBarTextures(Dictionary< ResourceType, Texture2D > images) {
			resourceHealthBarTextures = images;
		}

	}
}
