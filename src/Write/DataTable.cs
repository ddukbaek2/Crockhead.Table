using System.Collections.Generic;


namespace Crockhead.Table
{
	/// <summary>
	/// 데이터 테이블.
	/// </summary>
	public class DataTable
	{
		/// <summary>
		/// 그룹.
		/// </summary>
		public string Group { set; get; }

		/// <summary>
		/// 이름.
		/// </summary>
		public string Name { set; get; }

		/// <summary>
		/// 필드 목록.
		/// </summary>
		public List<string> Fields { set; get; }

		/// <summary>
		/// 타입 목록.
		/// </summary>
		public List<string> Types { set; get; }

		/// <summary>
		/// 주석 목록.
		/// </summary>
		public List<string> Comments { set; get; }

		/// <summary>
		/// 옵션 목록.
		/// </summary>
		public List<string> Options { set; get; }

		/// <summary>
		/// 레코드 목록.
		/// </summary>
		public List<List<string>> Records { set; get; }

		/// <summary>
		/// 생성됨.
		/// </summary>
		public DataTable()
		{
			Group = string.Empty;
			Name = string.Empty;
			Fields = new List<string>();
			Types = new List<string>();
			Comments = new List<string>();
			Options = new List<string>();
			Records = new List<List<string>>();
		}
	}
}