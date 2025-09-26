using System;
using System.Collections.Generic;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블 인터페이스.
	/// <para>기록 객체를 보관하는 컬렉션.</para>
	/// </summary>
	public interface ITable
	{
		/// <summary>
		/// 레코드 갯수 프로퍼티.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// 모든 레코드 제거.
		/// </summary>
		void Clear();

		/// <summary>
		/// 레코드 추가.
		/// </summary>
		void Add(IRecordable record);

		/// <summary>
		/// 레코드 범위 추가.
		/// </summary>
		void AddRange(IEnumerable<IRecordable> records);

		/// <summary>
		/// 레코드 제거.
		/// </summary>
		bool Remove(int id);

		/// <summary>
		/// 레코드 포함 여부 반환.
		/// </summary>
		bool Contains(int id);

		/// <summary>
		/// 레코드 포함 여부 반환.
		/// </summary>
		bool Contains(Predicate<IRecordable> predicate);

		/// <summary>
		/// 조건에 맞는 레코드 반환.
		/// </summary>
		IRecordable Find(int id);

		/// <summary>
		/// 조건에 맞는 레코드 반환.
		/// </summary>
		IRecordable Find(Predicate<IRecordable> predicate);

		/// <summary>
		/// 조건에 맞는 모든 레코드 반환.
		/// </summary>
		List<IRecordable> FindAll(Predicate<IRecordable> predicate);

		/// <summary>
		/// 모든 레코드 반환.
		/// </summary>
		List<IRecordable> All();
	}
}