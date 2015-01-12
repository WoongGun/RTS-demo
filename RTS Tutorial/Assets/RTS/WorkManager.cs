using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTS {
	public static class WorkManager {

		/// <summary>
		/// Calculates the selection box for the object based on bounds and playing area.
		/// </summary>
		/// <returns>The selection box.</returns>
		/// <param name="selectionBounds">Selection bounds.</param>
		/// <param name="playingArea">Playing area.</param>
		public static Rect CalculateSelectionBox(Bounds selectionBounds, Rect playingArea) {
			//shorthand for the coordinates of the centre of the selection bounds
			float cx = selectionBounds.center.x;
			float cy = selectionBounds.center.y;
			float cz = selectionBounds.center.z;
			//shorthand for the coordinates of the extents of the selection bounds
			float ex = selectionBounds.extents.x;
			float ey = selectionBounds.extents.y;
			float ez = selectionBounds.extents.z;
			
			//Determine the screen coordinates for the corners of the selection bounds
			List < Vector3 > corners = new List < Vector3 >();
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex, cy+ey, cz+ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex, cy+ey, cz-ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex, cy-ey, cz+ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex, cy+ey, cz+ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx+ex, cy-ey, cz-ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex, cy-ey, cz+ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex, cy+ey, cz-ez)));
			corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx-ex, cy-ey, cz-ez)));
			
			//Determine the bounds on screen for the selection bounds
			Bounds screenBounds = new Bounds(corners[0], Vector3.zero);
			for(int i = 1; i < corners.Count; i++) {
				screenBounds.Encapsulate(corners[i]);
			}
			
			//Screen coordinates start in the bottom left corner, rather than the top left corner
			//this correction is needed to make sure the selection box is drawn in the correct place
			float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
			float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
			float selectBoxWidth = 2 * screenBounds.extents.x;
			float selectBoxHeight = 2 * screenBounds.extents.y;
			
			return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
		}

		/// <summary>
		/// Finds the hit object, used for finding the object under the cursor by using raycast.
		/// </summary>
		/// <returns>The hit object.</returns>
		/// <param name="origin">Origin.</param>
		public static GameObject FindHitObject(Vector3 origin) {
			Ray ray = Camera.main.ScreenPointToRay(origin);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
			return null;
		}

		/// <summary>
		/// Finds the hit point of the object under the cursor.
		/// </summary>
		/// <returns>The hit point.</returns>
		/// <param name="origin">Origin.</param>
		public static Vector3 FindHitPoint(Vector3 origin) {
			Ray ray = Camera.main.ScreenPointToRay(origin);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) return hit.point;
			return ResourceManager.InvalidPosition;
		}
	}
}