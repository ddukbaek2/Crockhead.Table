using Crockhead.Core;
using Crockhead.Logging;
using System;
using System.Collections.Generic;


namespace Crockhead.Table
{
	/// <summary>
	/// 공유 테이블.
	/// </summary>
	public class SharedTable<TClass, TRecordable, TRecordArrayReader> : SharedClass<TClass>
		where TClass : SharedTable<TClass, TRecordable, TRecordArrayReader>, new()
		where TRecordable : IRecordable
		where TRecordArrayReader : IRecordArrayReader<TRecordable>
	{
		/// <summary>
		/// 컬렉션.
		/// </summary>
		private Table<TRecordable> m_Collection;

		/// <summary>
		/// 테이블 클래스 타입 이름.
		/// </summary>
		private string m_TableClassName;

		/// <summary>
		/// 리더.
		/// </summary>
		private TRecordArrayReader m_Reader;

		/// <summary>
		/// 로거.
		/// </summary>
		private Logger m_Logger;

		/// <summary>
		/// 컬렉션 프로퍼티.
		/// </summary>
		public Table<TRecordable> Collection => m_Collection;

		/// <summary>
		/// 리더 프로퍼티.
		/// </summary>
		public TRecordArrayReader Reader => m_Reader;

		/// <summary>
		/// 로거 프로퍼티.
		/// </summary>
		public Logger Logger => m_Logger;

		/// <summary>
		/// 생성됨.
		/// </summary>
		public SharedTable() : base()
		{
			var type = typeof(TClass);
			m_Collection = null;
			m_TableClassName = type.Name;
			m_Reader = default;
			m_Logger = new Logger($"{m_TableClassName}_Logger");
		}

		/// <summary>
		/// 생성됨.
		/// </summary>
		protected override void OnCreate(params object[] arguments)
		{
			base.OnCreate(arguments);
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
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			var operation = reader.Read();
			if (operation.IsSucceeded)
			{
				var records = reader.Records;
				m_Collection = new Table<TRecordable>(records);
				m_Collection.AddRange(records);
				m_Logger?.Log($"[{m_TableClassName}] Load Complete.");
				OnLoaded(true);
			}
			else
			{
				m_Logger?.Log($"[{m_TableClassName}] Not Found AssetPath Attribute.");
				OnLoaded(false);
			}
		}

		/// <summary>
		/// 테이블 로드 됨.
		/// </summary>
		protected virtual void OnLoaded(bool successed)
		{
		}

		/// <summary>
		/// 리더 설정.
		/// </summary>
		public void SetReader(TRecordArrayReader reader)
		{
			m_Reader = reader;
		}

		/// <summary>
		/// 로드.
		/// </summary>
		public void Load(TRecordArrayReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			SetReader(reader);
			OnLoad(Reader);
		}

		/// <summary>
		/// 로드.
		/// </summary>
		public void Load()
		{
			OnLoad(Reader);
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