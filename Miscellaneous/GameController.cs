using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	/* Controls overarching game effects */

	[SerializeField] private Texture2D cursorSprite; //new sprite to use as the in game cursor

	void Start () {
		Cursor.SetCursor (cursorSprite, Vector2.zero, CursorMode.Auto);
	}
}
