using Crockhead.Core;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블.
	/// <para>기록 객체를 보관하는 컬렉션.</para>
	/// <para>모든 레코드는 동일 컬렉션안에서 정수형 고유식별자를 들고 있으며 해당 레코드의 고유식별자를 키로 사용.</para>
	/// <para>IEnumerable.Record 인터페이스 구현체.</para>
	/// </summary>
	public class Table : Disposable, ITable, IEnumerable<IRecordable>
	{
		/// <summary>
		/// 레코드 목록.
		/// </summary>
		private SortedDictionary<int, IRecordable> m_Records;

		/// <summary>
		/// 갯수 프로퍼티.
		/// </summary>
		public int Count => m_Records.Count;

		/// <summary>
		/// 레코드 목록 프로퍼티.
		/// </summary>
		public IEnumerable<IRecordable> Values => m_Records.Values;

		/// <summary>
		/// 생성됨.
		/// </summary>
		public Table() : base()
		{
			m_Records = new SortedDictionary<int, IRecordable>();
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose(bool explicitDisposing)
		{
		}

		/// <summary>
		/// 모든 레코드 제거.
		/// </summary>
		public void Clear()
		{
			m_Records.Clear();
		}

		/// <summary>
		/// 레코드 추가.
		/// <para>고유식별자가 기존에 있는 경우는 교체.</para>
		/// </summary>
		public void Add(IRecordable record)
		{
			if (record == null)
				return;

			m_Records[record.Id] = record;
		}

		/// <summary>
		/// 레코드 범위 추가.
		/// <para>고유식별자가 기존에 있는 경우는 교체.</para>
		/// </summary>
		public void AddRange(IEnumerable<IRecordable> records)
		{
			if (records == null)
				return;

			foreach (var record in records)
			{
				m_Records[record.Id] = record;
			}
		}

		/// <summary>
		/// 레코드 제거.
		/// </summary>
		public bool Remove(int id)
		{
			return m_Records.Remove(id);
		}

		/// <summary>
		/// 레코드 포함 여부 반환.
		/// </summary>
		public bool Contains(int id)
		{
			return m_Records.ContainsKey(id);
		}

		/// <summary>
		/// 레코드 포함 여부 반환.
		/// </summary>
		public bool Contains(Predicate<IRecordable> predicate)
		{
			foreach (var record in m_Records.Values)
			{
				if (predicate?.Invoke(record) ?? false)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 조건에 맞는 레코드를 반환.
		/// </summary>
		public IRecordable Find(int id)
		{
			return m_Records[id];
		}

		/// <summary>
		/// 조건에 맞는 레코드를 반환.
		/// </summary>
		public IRecordable Find(Predicate<IRecordable> predicate)
		{
			foreach (var record in m_Records.Values)
			{
				if (predicate?.Invoke(record) ?? false)
					return record;
			}
			return default;
		}

		/// <summary>
		/// 조건에 맞는 모든 레코드를 반환.
		/// </summary>
		public List<IRecordable> FindAll(Predicate<IRecordable> predicate)
		{
			var result = new List < IRecordable >();
			foreach (var record in m_Records.Values)
			{
				if (predicate?.Invoke(record) ?? false)
					result.Add(record);
			}
			return result;
		}

		/// <summary>
		/// 모든 레코드 반환.
		/// </summary>
		public List<IRecordable> All()
		{
			return new List<IRecordable>(m_Records.Values);
		}

		/// <summary>
		/// 열거 제공자 반환.
		/// <para>IEnumerable.Record 인터페이스 구현.</para>
		/// </summary>
		IEnumerator<IRecordable> IEnumerable<IRecordable>.GetEnumerator()
		{
			return m_Records.Values.GetEnumerator();
		}

		/// <summary>
		/// 열거 제공자 반환.
		/// <para>IEnumerable 인터페이스 구현.</para>
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_Records.Values.GetEnumerator();
		}

		/// <summary>
		/// 생성.
		/// </summary>
		public static Table Create(IRecordable[] records = null)
		{
			var obj = Reflections.CreateInstance<Table>();
			obj.AddRange(records);
			return obj;
		}
	}
}