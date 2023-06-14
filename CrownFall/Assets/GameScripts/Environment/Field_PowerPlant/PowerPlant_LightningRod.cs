using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class PowerPlant_LightningRod : MonoBehaviour
{
	//---------------------------------------
	[Header("INFO")]
	[SerializeField]
	private PowerPlant_LightningRod_Info _info = null;

	[Header("ROD OBJECTS")]
	[SerializeField]
	private Light _rod_Light = null;

	//---------------------------------------
	private Vector3 _rod_PivotPos;
	private ParticleSystem _rod_Particle = null;

	public Light Rod_Light { get { return _rod_Light; } }
	public Vector3 PivotPos { get { return _rod_PivotPos; } }

	//---------------------------------------
	private void Start()
	{
		_rod_PivotPos = _rod_Light.transform.position;
        
		_rod_Particle = HT.Utils.Instantiate(_info._rodParticle);
		_rod_Particle.transform.SetParent(gameObject.transform);
		_rod_Particle.transform.position = _rod_Light.transform.position;
		_rod_Particle.transform.localScale = _rod_Light.transform.localScale;
		_rod_Particle.transform.rotation = _rod_Light.transform.rotation;
	}

	public void ActivateLight(float fTime, float fInten, float fRange, bool bShowParticle)
	{
		_rod_Light.intensity = fInten;
		_rod_Light.range = fRange;

		StopAllCoroutines();
		StartCoroutine(ActivateLight_Internal(fTime, bShowParticle));
	}

	//---------------------------------------
	private IEnumerator ActivateLight_Internal(float fTime, bool bShowParticle)
	{
		if (bShowParticle)
			_rod_Particle.Play();

		if (_rod_Light.enabled)
		{
			_rod_Light.gameObject.SetActive(true);

			float fDefTime = fTime;
			float fDefIntensity = _rod_Light.intensity;
			while (fTime > 0.0f)
			{
				fTime -= HT.TimeUtils.GameTime;
				_rod_Light.intensity = fDefIntensity * (fTime / fDefTime);

				yield return new WaitForEndOfFrame();
			}

			_rod_Light.gameObject.SetActive(false);
		}
	}
}


/////////////////////////////////////////
//---------------------------------------