using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;

public class LoadingPanel : GameShow
{
	private sealed class _delayClose_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		internal Action callBack;

		internal LoadingPanel _this;

		internal object _current;

		internal bool _disposing;

		internal int _PC;

		object IEnumerator<object>.Current
		{
			get
			{
				return this._current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._current;
			}
		}

		public _delayClose_c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this._PC;
			this._PC = -1;
			switch (num)
			{
			case 0u:
				this._current = new WaitForSeconds(this._this.m_ShowTime);
				if (!this._disposing)
				{
					this._PC = 1;
				}
				return true;
			case 1u:
				this._this.Close(this.callBack, 0f);
				this._PC = -1;
				break;
			}
			return false;
		}

		public void Dispose()
		{
			this._disposing = true;
			this._PC = -1;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	[SerializeField]
	private float m_ShowTime = 0.5f;

	public override void Open(Action callBack = null, float dealy = 0f)
	{
		base.Open(callBack, dealy);
		MonoSingleton<GamePlayManager>.Instance.enabled = false;
	}

	public override void Close(Action callBack = null, float dealy = 0f)
	{
		base.Close(callBack, dealy);
		MonoSingleton<GamePlayManager>.Instance.enabled = true;
	}

	public void Show(Action callBack = null, float dealy = 0f)
	{
		this.Open(null, 0f);
		base.StopCoroutine("delayClose");
		base.StartCoroutine(this.delayClose(callBack));
	}

	private IEnumerator delayClose(Action callBack = null)
	{
		LoadingPanel._delayClose_c__Iterator0 _delayClose_c__Iterator = new LoadingPanel._delayClose_c__Iterator0();
		_delayClose_c__Iterator.callBack = callBack;
		_delayClose_c__Iterator._this = this;
		return _delayClose_c__Iterator;
	}
}
