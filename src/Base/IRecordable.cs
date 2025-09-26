using System;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블 1건에 대한 데이터 인터페이스.
	/// </summary>
	public interface IRecordable
	{
		/// <summary>
		/// 고유 식별자.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// 필드 갯수.
		/// </summary>
		int GetFieldCount();

		/// <summary>
		/// 필드 이름.
		/// </summary>
		string GetFieldName(int index);

		/// <summary>
		/// 필드 타입.
		/// </summary>
		Type GetFieldType(int index);

		/// <summary>
		/// 필드 값.
		/// </summary>
		object GetFieldValue(int index);
	}
}