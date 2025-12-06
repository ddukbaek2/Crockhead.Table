using Crockhead.Core;
using Crockhead.Logging;
using System;
using System.Collections.Generic;


namespace Crockhead.Table
{
	/// <summary>
	/// 공유 테이블.
	/// </summary>
	public class SharedTable<TClass, TRecordable> : SharedClass<TClass>
		where TClass : SharedTable<TClass, TRecordable>, new()
		where TRecordable : IRecordable
	{
		/// <summary>
		/// 컬렉션.
		/// </summary>
		protected Table<TRecordable> m_Collection;

		/// <summary>
		/// 컬렉션 프로퍼티.
		/// </summary>
		public Table<TRecordable> Collection => m_Collection;

		/// <summary>
		/// 생성됨.
		/// </summary>
		public SharedTable() : base()
		{
		}

		/// <summary>
		/// 생성됨.
		/// </summary>
		protected override void OnCreate()
		{
			base.OnCreate();

			m_Collection = new Table<TRecordable>();
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose()
		{
			Disposables.SafeDispose(ref m_Collection);

			base.OnDispose();
		}

		/// <summary>
		/// 테이블 로드 됨.
		/// </summary>
		protected virtual void OnTableDidLoad()
		{
		}

		/// <summary>
		/// 로드.
		/// </summary>
		public virtual void LoadTable(IRecordArrayReader<TRecordable> reader)
		{
			try
			{
				if (reader == null)
					throw new ArgumentNullException(nameof(reader));

				var records = reader.Records;
				m_Collection.AddRange(records);
				OnTableDidLoad();
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 레코드 포함 여부 반환.
		/// </summary>
		public bool Contains(int id)
		{
			return m_Collection.Contains(id);
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