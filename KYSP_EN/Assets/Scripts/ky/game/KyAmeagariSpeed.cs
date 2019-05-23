using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�J������v�̃X�s�[�h���[�^�[�p�X�N���v�g�B
/// </summary>
public class KyAmeagariSpeed : KyScriptObject {

	protected override void Start () {
		base.Start();
		Transform car = transform.parent.Find("car");
		if (car != null) {
			CarObject = car.gameObject;
		}
		mScriptDriven = true;
		UpdateSpeed(DefaultSpeed);
	}
	
	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		float speed = mSpeed;
		//	�����x�ɂ�鑬�x�ω�
		speed += Acceleration * DeltaTime;
		//	�h���b�O�ɂ�鑬�x�ω�
		float dy = mInput.DeltaPosition.y;
		if (dy != 0) {
			speed += dy / GaugeLength * DragFactor;
		}
		speed = Mathf.Clamp01(speed);
		if (speed != mSpeed) {
			UpdateSpeed(speed);
		}
		//	�Ԃ̈ړ�
		Vector3 pos = CarObject.transform.localPosition;
		pos.y += mSpeed * SpeedFactor * DeltaTime;
		CarObject.transform.localPosition = pos;
	}

	private void UpdateSpeed(float speed) {
		mSpeed = speed;
		Vector2 size = GaugeSprite.Size;
		size.y = mSpeed * GaugeLength;
		GaugeSprite.Size = size;
		GaugeSprite.UpdateAll();
		//KyAudioManager.Instance.Play("bgm_car", true, mSpeed, 1, 0);
		CommandManager.SetVariable("speed", mSpeed);
	}

	public float DefaultSpeed = 0.5f;	//	�����X�s�[�h�B
	public float Acceleration = 0.2f;	//	�P�b�Ԃɑ�������X�s�[�h�ʁB
	public float SpeedFactor = 30.0f;	//	�X�s�[�h�ő厞�̎Ԃ̃s�N�Z���ړ��ʁB
	public float DragFactor = 0.5f;
	public float GaugeLength = 120.0f;	//	�X�s�[�h���[�^�[�̍ő咷�B
	public GameObject CarObject = null;
	public SpriteSimple GaugeSprite = null;

	private bool mEnabled = true;
	private float mSpeed;	//	�X�s�[�h���[�^�̗ʁB0���Œ�A1���ő��B
	private KyInputDrag mInput = new KyInputDrag();	//	�h���b�O���̓��W���[��
}
