using System;
using System.Collections;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTComponent
	{
		//---------------------------------------
		public virtual void Initialize() { }
		public virtual void Tick(float fDelta) { }
		public virtual void FixedTick(float fDelta) { }
		public virtual void OnDestroy() { }

		//---------------------------------------
		//public override void Initialize() { }
		//public override void Tick(float fDelta) { }
		//public override void FixedTick(float fDelta) { }
		//public override void OnDestroy() { }
	}


	/////////////////////////////////////////
	//---------------------------------------
}