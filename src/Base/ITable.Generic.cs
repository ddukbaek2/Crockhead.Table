using System;
using System.Collections.Generic;


namespace Crockhead.Table
{
	/// <summary>
	/// 제네릭 테이블 인터페이스.
	/// <para>기록 객체를 보관하는 컬렉션.</para>
	/// </summary>
	public interface ITable<TRecordable> : ITable where TRecordable : IRecordable
	{
		/// <summary>
		/// 레코드 추가.
		/// </summary>
		void Add(TRecordable record);

		/// <summary>
		/// 레코드 범위 추가.
		/// </summary>
		void AddRange(IEnumerable<TRecordable> records);

		/// <summary>
		/// 레코드 포함 여부 반환.
		/// </summary>
		bool Contains(Predicate<TRecordable> predicate);

		/// <summary>
		/// 조건에 맞는 레코드 반환.
		/// </summary>
		new TRecordable Find(int id);

		/// <summary>
		/// 조건에 맞는 레코드 반환.
		/// </summary>
		TRecordable Find(Predicate<TRecordable> predicate);

		/// <summary>
		/// 조건에 맞는 모든 레코드 반환.
		/// </summary>
		List<TRecordable> FindAll(Predicate<TRecordable> predicate);

		/// <summary>
		/// 모든 레코드 반환.
		/// </summary>
		new List<TRecordable> All();
	}
}