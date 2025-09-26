using Crockhead.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace Crockhead.Table.src.Read
{
	/// <summary>
	/// 레코드 배열 파일 리더.
	/// </summary>
	public class RecordArrayFileReader<TRecordable> : Disposable, IRecordArrayReader<TRecordable> where TRecordable : IRecordable
	{
		/// <summary>
		/// 경로.
		/// </summary>
		private string m_FilePath;

		/// <summary>
		/// 레코드 목록.
		/// </summary>
		private List<TRecordable> m_Records;

		/// <summary>
		/// 읽어들인 레코드 목록 프로퍼티.
		/// </summary>
		public TRecordable[] Records => m_Records.ToArray();

		/// <summary>
		/// 생성됨.
		/// </summary>
		public RecordArrayFileReader(string filePath) : base()
		{
			m_FilePath = filePath;
			m_Records = new List<TRecordable>();
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose(bool explicitDisposing)
		{
		}

		/// <summary>
		/// 읽기.
		/// </summary>
		public Operation<TRecordable[]> Read()
		{
			void OnOperation(Operation<TRecordable[]> operation)
			{
				try
				{
					var json = File.ReadAllText(m_FilePath);
					var records = JsonConvert.DeserializeObject<TRecordable[]>(json);
					m_Records.Clear();
					if (records != null && records.Length > 0)
						m_Records.AddRange(records);
					operation.Success(Records);

				}
				catch (OperationCanceledException)
				{
					operation.Cancel();
				}
				catch (Exception exception)
				{
					//Debug.LogError($"[RecordArrayReader] '{assetPath}' JSON File Load Failed.");
					operation.Fail(exception);
					throw;
				}
			}

			var operation = new Operation<TRecordable[]>(OnOperation);
			operation.Start();
			return operation;
		}
	}
}
