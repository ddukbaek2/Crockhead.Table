using Crockhead.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Crockhead.Table
{
	/// <summary>
	/// 레코드 배열 파일 로더.
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
		public IEnumerable<TRecordable> Records => m_Records;

		/// <summary>
		/// 생성됨.
		/// </summary>
		public RecordArrayFileReader(object sharedTableInstance) : base()
		{
			if (sharedTableInstance == null)
				throw new ArgumentNullException(nameof(sharedTableInstance));

			var sharedTableType = sharedTableInstance.GetType();
			if (!Reflections.TryGetAttribute<FilePathAttribute>(sharedTableType, out var filePathAttribute))
				throw new ArgumentNullException(nameof(filePathAttribute));

			InternalInitialize(filePathAttribute);
		}

		/// <summary>
		/// 생성됨.
		/// </summary>
		public RecordArrayFileReader(FilePathAttribute filePathAttribute) : base()
		{
			InternalInitialize(filePathAttribute);
		}

		/// <summary>
		/// 생성됨.
		/// </summary>
		public RecordArrayFileReader(string filePath) : base()
		{
			InternalInitialize(filePath);
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose(bool explicitDisposing)
		{
		}

		/// <summary>
		/// 초기화.
		/// </summary>
		private void InternalInitialize(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentNullException(nameof(filePath));
			if (!File.Exists(filePath))
				throw new FileNotFoundException(nameof(filePath));

			m_FilePath = filePath;
			m_Records = new List<TRecordable>();
		}


		/// <summary>
		/// 초기화.
		/// </summary>
		private void InternalInitialize(FilePathAttribute filePathAttribute)
		{
			if (filePathAttribute == null)
				throw new ArgumentNullException(nameof(filePathAttribute));
			if (!filePathAttribute.IsEnabled)
				throw new InvalidOperationException(nameof(filePathAttribute.IsEnabled));

			var filePath = filePathAttribute.Value;
			InternalInitialize(filePath);
		}

		/// <summary>
		/// 불러오기.
		/// </summary>
		public IEnumerable<TRecordable> Read()
		{
			try
			{
				var json = File.ReadAllText(m_FilePath);
				var records = JsonConvert.DeserializeObject<TRecordable[]>(json);
				m_Records.Clear();
				if (records != null && records.Length > 0)
					m_Records.AddRange(records);
				return m_Records;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// 불러오기. (비동기)
		/// </summary>
		public async Task<IEnumerable<TRecordable>> ReadAsync()
		{
			try
			{				
				return await Task.Run(async () =>
				{
					var json = await File.ReadAllTextAsync(m_FilePath);
					var records = JsonConvert.DeserializeObject<TRecordable[]>(json);
					m_Records.Clear();
					if (records != null && records.Length > 0)
						m_Records.AddRange(records);
					return m_Records;
				});
			}
			catch
			{
				throw;
			}
		}


		///// <summary>
		///// 불러오기.
		///// </summary>
		//public Operation<TRecordable[]> Read()
		//{
		//	void OnOperation(Operation<TRecordable[]> operation)
		//	{
		//		try
		//		{
		//			var json = File.ReadAllText(m_FilePath);
		//			var records = JsonConvert.DeserializeObject<TRecordable[]>(json);
		//			m_Records.Clear();
		//			if (records != null && records.Length > 0)
		//				m_Records.AddRange(records);
		//			operation.Success(Records);

		//		}
		//		catch (OperationCanceledException)
		//		{
		//			operation.Cancel();
		//		}
		//		catch (Exception exception)
		//		{
		//			//Debug.LogError($"[RecordArrayReader] '{assetPath}' JSON File Read Failed.");
		//			operation.Fail(exception);
		//			throw;
		//		}
		//	}

		//	var operation = new Operation<TRecordable[]>(OnOperation);
		//	operation.Start();
		//	return operation;
		//}
	}
}
