namespace Crockhead.Table
{
	/// <summary>
	/// 원시 테이블.
	/// </summary>
	public class RawTable
	{
		/// <summary>
		/// 파일 경로.
		/// </summary>
		public string FilePath { set; get; }

		/// <summary>
		/// 그룹.
		/// </summary>
		public string Group { set; get; }

		/// <summary>
		/// 이름.
		/// </summary>
		public string Name { set; get; }

		/// <summary>
		/// 2차원 배열 데이터.
		/// </summary>
		public string[][] Value { set; get; }

		/// <summary>
		/// 생성됨.
		/// </summary>
		public RawTable()
		{
			FilePath = string.Empty;
			Group = string.Empty;
			Name = string.Empty;
			Value = null;
		}
	}
}