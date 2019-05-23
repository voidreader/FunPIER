using UnityEngine;
using System.Collections;

public class SpriteText : Sprite {

	void Awake() {
		mTextMesh = gameObject.AddComponent<TextMesh>();
		gameObject.AddComponent<MeshRenderer>();
		UpdateFont();
		UpdateAll();
	}

	public void UpdateFont() {
		mTextMesh.font = Font;
		GetComponent<Renderer>().material = Material;
	}

	public override void UpdateAll() {
		mTextMesh.text = Text;
		mTextMesh.characterSize = CharacterSize;

		mTextMesh.anchor = (TextAnchor)((int)AnchorX + (2 - (int)AnchorY) * 3);
		if (Vertical) {
			transform.rotation = Quaternion.Euler(0, 0, -90);
			mTextMesh.lineSpacing = 0.7f;
		} else {
			transform.rotation = Quaternion.Euler(0, 0, 0);
			mTextMesh.lineSpacing = 1.0f;
		}
		GetComponent<Renderer>().material.color = Color;
	}

	public string Text = "";
	public int CharacterSize = 16;
	public bool Vertical = true;
	public Color Color = new Color(1, 1, 1, 1);
	public Font Font = null;
	public Material Material = null;
	public SpriteAnchor AnchorX = SpriteAnchor.Middle;
	public SpriteAnchor AnchorY = SpriteAnchor.Middle;

	protected TextMesh mTextMesh = null;
}
