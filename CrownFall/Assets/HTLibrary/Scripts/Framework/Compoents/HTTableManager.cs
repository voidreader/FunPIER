using System;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// Key값에 의해 찾아진 Table들을 담고있는 Class입니다.
	/// 사용 후에 해제되면서 자동으로 원본 Table을 Memory에서 해제합니다.
	/// </summary>
	public class HTSmartTable
	{
		//---------------------------------------
		public HTSmartTable(HTTableManager pManager)
		{
			_tableManager = pManager;
		}

		~HTSmartTable()
		{
			for (int nInd = 0; nInd < _tableCount; ++nInd)
				_tableManager.ReleaseTable(_tables[nInd]);
		}

		//---------------------------------------
		private HTTableManager _tableManager = null;

		private List<HTTable> _tables = null;
		private int _tableCount = 0;

		public int Count { get { return _tableCount; } }
		public HTTable this [int nInd] { get { return GetTable(nInd); } }

		//---------------------------------------
		public void AddTable(HTTable pTable)
		{
			if (_tables == null)
				_tables = new List<HTTable>();

			_tables.Add(pTable);
			++_tableCount;
		}

		public HTTable GetTable(int nInd)
		{
			return (_tables != null)? _tables[nInd] : null;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// Project Define의 Table 목록을 자동으로 Load해주는 Class입니다.
	/// </summary>
	public sealed class HTTableManager : HTComponent
	{
		//---------------------------------------
		private UserTableSetting[] _userTables = null;
		public UserTableSetting[] UserTables { get { return _userTables; } }

		private Action _onLoadTableComplete = null;

		//---------------------------------------
		public override void Initialize()
		{
			GEnv pGEnv = HTFramework.Instance.RegistGEnv;
			_userTables = new UserTableSetting[pGEnv._userTableSettings.Length];
			for (int nInd = 0; nInd < pGEnv._userTableSettings.Length; ++nInd)
			{
				string szTableParam = pGEnv._userTableSettings[nInd];
				_userTables[nInd] = new UserTableSetting();
				_userTables[nInd].Initialize(szTableParam);
			}

			Utils.SafeInvoke(_onLoadTableComplete);
		}

		public override void Tick(float fDelta)
		{
		}

		public override void FixedTick(float fDelta)
		{
		}

		public override void OnDestroy()
		{
		}

		//---------------------------------------
		/// <summary>
		/// 해당 Key 값으로 등록 된 Table을 Return합니다.
		/// </summary>
		public HTSmartTable FindUserTable(string szKey)
		{
			for (int nInd = 0; nInd < _userTables.Length; ++nInd)
			{
				if (_userTables[nInd]._key != szKey)
					continue;

				if (_userTables[nInd]._table == null)
				{
					HTDebug.PrintLog(eMessageType.Error, "[TableMan] Table has released yet!");
					continue;
				}

				HTSmartTable pSmartTable = new HTSmartTable(this);
				pSmartTable.AddTable(_userTables[nInd]._table);
				return pSmartTable;
			}

			return null;
		}

		/// <summary>
		/// 해당 Key 값으로 등록 된 Table을 모두 Return합니다.
		/// </summary>
		public HTSmartTable FindUserTableAll(string szKey)
		{
			HTSmartTable pSmartTable = null;

			for (int nInd = 0; nInd < _userTables.Length; ++nInd)
			{
				if (_userTables[nInd]._key != szKey)
					continue;

				if (_userTables[nInd]._table == null)
				{
					HTDebug.PrintLog(eMessageType.Error, "[TableMan] Table has released yet!");
					continue;
				}

				if (pSmartTable == null)
					pSmartTable = new HTSmartTable(this);

				pSmartTable.AddTable(_userTables[nInd]._table);
			}

			if (pSmartTable == null || pSmartTable.Count == 0)
				HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTTableMan] There's no table by id {0}!", szKey));

			return pSmartTable;
		}

		//---------------------------------------
		public void ReleaseTable(HTTable pTable)
		{
			for (int nInd = 0; nInd < _userTables.Length; ++nInd)
			{
				if (_userTables[nInd]._table == pTable)
					_userTables[nInd]._table = null;
			}
		}

		//---------------------------------------
	}

	/////////////////////////////////////////
	//---------------------------------------
}
