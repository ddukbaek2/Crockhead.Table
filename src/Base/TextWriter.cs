using Crockhead.Core;
using System.Text;


namespace Crockhead.Table
{
	/// <summary>
	/// 코드 라이터.
	/// </summary>
	public class TextWriter : Disposable
	{
		/// <summary>
		/// 문자열 처리기.
		/// </summary>
		private StringBuilder m_StringBuilder;

		/// <summary>
		/// 들여쓰기 수준.
		/// </summary>
		private int m_IndentLevel;

		/// <summary>
		/// 들여쓰기 수준 프로퍼티.
		/// </summary>
		public int IndentLevel { set => m_IndentLevel = value; get => m_IndentLevel; }

		/// <summary>
		/// 생성됨.
		/// </summary>
		public TextWriter() : base()
		{
			m_StringBuilder = new StringBuilder();
			m_IndentLevel = 0;
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose(bool explicitDisposing)
		{
		}

		/// <summary>
		/// 들여쓰기 수준 증가.
		/// </summary>
		public void IncreaseIndentLevel()
		{
			++m_IndentLevel;
		}

		/// <summary>
		/// 들여쓰기 수준 감소.
		/// </summary>
		public void DecreaseIndentLevel()
		{
			++m_IndentLevel;
		}

		/// <summary>
		/// 비우기.
		/// </summary>
		public void Clear()
		{
			m_StringBuilder.Clear();
			m_IndentLevel = 0;
		}

		/// <summary>
		/// 들여쓰기.
		/// </summary>
		public void WriteIndentLevel()
		{
			for (var i = 0; i < m_IndentLevel; ++i)
				m_StringBuilder.Append("\t");
		}

		/// <summary>
		/// 쓰기 시작. (들여쓰기)
		/// </summary>
		public void BeginWrite()
		{
			WriteIndentLevel();
		}

		/// <summary>
		/// 쓰기.
		/// </summary>
		public void Write(string value)
		{
			m_StringBuilder.Append(value);
		}

		/// <summary>
		/// 쓰기 종료. (개행)
		/// </summary>
		public void EndWrite()
		{
			m_StringBuilder.AppendLine();
		}

		/// <summary>
		/// 한줄 쓰기.
		/// </summary>
		public void WriteLine()
		{
			BeginWrite();
			EndWrite();
		}

		/// <summary>
		/// 한줄 쓰기.
		/// </summary>
		public void WriteLine(string value)
		{
			BeginWrite();
			Write(value);
			EndWrite();
		}

		/// <summary>
		/// 문자열 반환.
		/// </summary>
		public override string ToString()
		{
			return m_StringBuilder.ToString();
		}
	}
}