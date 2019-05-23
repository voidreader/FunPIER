using UnityEngine;
using System.Collections;

public class KySpriteNumber : MonoBehaviour {

	public enum NumberAnchor {
		Left = 0,
		Center,
		Right,
	}

	void Awake() {
		CreateDigitSprites();
		UpdateNumber();
	}

	void Start () {
	}
	
	public void Update () {
		if (Number != mOldNumber) {
			mOldNumber = Number;
			UpdateNumber();
		}
	}

	private void CreateDigitSprites() {
		Assert.AssertNotNull(DigitsPrefab);
		mDigitSprites = new KySpriteAnimation[MaxDigitLength];
		for (int i = 0; i < MaxDigitLength; ++i) {
			GameObject obj = (GameObject)Instantiate(DigitsPrefab);
			obj.transform.parent = transform;
			obj.layer = gameObject.layer;
			KySpriteAnimation sprite = obj.GetComponent<KySpriteAnimation>();
			Assert.AssertNotNull(sprite);
			mDigitSprites[i] = sprite;
			obj.active = false;
		}
	}

	private void UpdateNumber() {
		Number = Mathf.Clamp(Number, MinNumber, MaxNumber);
		mDigitLength = 0;
		if (Number == 0) {
			mDigitLength = 1;
		} else {
			mDigitLength = (int)Mathf.Log10(Number) + 1;
		}
		int index = 0;
		int num = Mathf.Abs(Number);
		while (num > 0 || index == 0) {
			mDigitSprites[index].AnimationIndex = (num % 10);
			num /= 10;
			++index;
		}
		
		if (mDigitLength != mOldDigitLength) {
			mOldDigitLength = mDigitLength;
			index = 0;
			mAnchorX =
				Anchor == NumberAnchor.Left ? mDigitLength * DigitWidth :
				Anchor == NumberAnchor.Center ? mDigitLength * DigitWidth / 2.0f :
				Anchor == NumberAnchor.Right ? 0 :
				0;
			for (; index < mDigitLength; ++index) {
				KySpriteAnimation sprite = mDigitSprites[index];
				sprite.gameObject.active = true;
				Vector3 position = sprite.transform.localPosition;
				position.x = mAnchorX + (- 1 - 2 * index) * DigitWidth / 2.0f;
				position.y = 0;
				position.z = 0;
				sprite.transform.localPosition = position;
			}
			for (; index < MaxDigitLength; ++index) {
				mDigitSprites[index].gameObject.active = false;
			}
		}
	}

	public float AnchorX {
		get { return mAnchorX; }
	}

	public int Number = 0;
	public int MinNumber = 0;
	public int MaxNumber = 9999;
	public int MaxDigitLength = 4;
	public int DigitWidth = 16;
	public NumberAnchor Anchor;
	public GameObject DigitsPrefab = null;
	
	private int mOldNumber = 0;
	private int mDigitLength = 0;
	private int mOldDigitLength = -1;
	private float mAnchorX;
	private KySpriteAnimation[] mDigitSprites = null;
}
