using System.Collections.Generic;
using System.Threading.Tasks;


namespace Crockhead.Table
{
	/// <summary>
	/// 레코드 배열 로더 인터페이스.
	/// </summary>
	public interface IRecordArrayReader<TRecordable> where TRecordable : IRecordable
	{
		/// <summary>
		/// 읽어들인 레코드 목록 프로퍼티.
		/// </summary>
		IEnumerable<TRecordable> Records { get; }

		/// <summary>
		/// 불러오기. (동기)
		/// </summary>
		IEnumerable<TRecordable> Read();

		/// <summary>
		/// 불러오기. (비동기)
		/// </summary>
		Task<IEnumerable<TRecordable>> ReadAsync();
	}
}