using Crockhead.Core;
using Crockhead.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Crockhead.Table
{
	/// <summary>
	/// 공유 테이블.
	/// </summary>
	public class SharedTable<TClass, TRecordable, TRecordArrayReader> : SharedClass<TClass>
		where TClass : SharedTable<TClass, TRecordable, IRecordArrayReader<TRecordable>>, new()
		where TRecordable : IRecordable
		where TRecordArrayReader : IRecordArrayReader<TRecordable>
	{
		/// <summary>
		/// 컬렉션.
		/// </summary>
		private Table<TRecordable> m_Collection;

		/// <summary>
		/// 로거.
		/// </summary>
		private Logger m_Logger;

		/// <summary>
		/// 컬렉션 프로퍼티.
		/// </summary>
		public Table<TRecordable> Collection => m_Collection;

		/// <summary>
		/// 생성됨.
		/// </summary>
		public SharedTable() : base()
		{
			m_Collection = null;
			m_Logger = null;
		}

		/// <summary>
		/// 로거 설정.
		/// </summary>
		public void SetLogger(Logger logger)
		{
			m_Logger = logger;
		}

		/// <summary>
		/// 생성됨.
		/// </summary>
		protected override void OnCreate()
		{
			Reload();
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose(bool explicitDisposing)
		{
			Disposables.SafeDispose(ref m_Collection);

			base.OnDispose(explicitDisposing);
		}

		/// <summary>
		/// 로드.
		/// </summary>
		protected virtual void OnLoad(TRecordArrayReader reader)
		{
			//	using var reader = new RecordArrayReader<TRecordable>(jsonAssetPath);
			//	reader.Read();
			var tableClassType = typeof(TRecordable);
			var operation = reader.Read();
			//operation.WaitForCompletion();
			Task.Run(() =>
			{
				operation.WaitForCompletion();
				if (operation.IsSucceeded)
				{
					var records = reader.Records;
					m_Collection = new Table<TRecordable>(records);
					m_Collection.AddRange(records);
					m_Logger?.Log($"[{tableClassType}] Load Complete.");
					OnLoaded(true);
				}
				else
				{
					m_Logger?.Log($"[{tableClassType}] Not Found AssetPath Attribute.");
					OnLoaded(false);
				}
			});
		}

		/// <summary>
		/// 테이블 로드 됨.
		/// </summary>
		protected virtual void OnLoaded(bool successed)
		{
		}

		/// <summary>
		/// 로드.
		/// </summary>
		public void Reload()
		{
			//OnLoad();
		}

		/// <summary>
		/// 조건에 맞는 레코드 반환.
		/// </summary>
		public TRecordable Find(int id)
		{
			return m_Collection.Find(id);
		}

		/// <summary>
		/// 조건에 맞는 레코드 반환.
		/// </summary>
		public TRecordable Find(Predicate<TRecordable> predicate)
		{
			return m_Collection.Find(predicate);
		}

		/// <summary>
		/// 조건에 맞는 모든 레코드 반환.
		/// </summary>
		public List<TRecordable> FindAll(Predicate<TRecordable> predicate)
		{
			return m_Collection.FindAll(predicate);
		}

		/// <summary>
		/// 모든 레코드 반환.
		/// </summary>
		public List<TRecordable> All()
		{
			return m_Collection.All();
		}
	}
}