using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블에서 사용되는 1건의 기록 데이터.
	/// <para>IRecordable 인터페이스 구현체.</para>
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Record : IRecordable
	{
		/// <summary>
		/// 고유 식별자 프로퍼티.
		/// </summary>
		[JsonProperty]
		public int Id { set; get; }

		/// <summary>
		/// 필드 갯수.
		/// </summary>
		int IRecordable.GetFieldCount()
		{
			return 1;
		}

		/// <summary>
		/// 필드 이름.
		/// </summary>
		string IRecordable.GetFieldName(int index)
		{
			switch (index)
			{
				case 0: return "Id";
				default: return null;
			}
		}

		/// <summary>
		/// 필드 타입.
		/// </summary>
		Type IRecordable.GetFieldType(int index)
		{
			switch (index)
			{
				case 0: return Id.GetType();
				default: return null;
			}
		}

		/// <summary>
		/// 필드 값.
		/// </summary>
		object IRecordable.GetFieldValue(int index)
		{
			switch (index)
			{
				case 0: return Id;
				default: return null;
			}
		}
	}
}