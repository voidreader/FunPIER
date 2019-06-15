using UnityEngine;
using System.Collections;

namespace PlatformerPro 
{
	/// <summary>
	/// Render particles on the provided layer.
	/// </summary>
	[RequireComponent (typeof(ParticleSystem))]
	public class RenderParticlesOnLayer : MonoBehaviour 
	{
		/// <summary>
		/// The layer to use for rendering.
		/// </summary>
		public string layer;

		/// <summary>
		/// The position in the layer.
		/// </summary>
		public int orderInLayer;

		void Start ()
		{
			// Set the sorting layer of the particle system.
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = layer;
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = orderInLayer;
		}
	}
}