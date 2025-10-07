using Crockhead.Core;


namespace Crockhead.Table
{
	/// <summary>
	/// 레코드 배열 리더.
	/// </summary>
	public interface IRecordArrayReader<TRecordable> where TRecordable : IRecordable
	{
		/// <summary>
		/// 읽어들인 레코드 목록 프로퍼티.
		/// </summary>
		TRecordable[] Records { get; }

		/// <summary>
		/// 읽기.
		/// </summary>
		Operation<TRecordable[]> Read();
	}
}