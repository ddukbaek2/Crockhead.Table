using Crockhead.Core;
using Crockhead.Logging;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블 생성기.
	/// <para>ExcelDataReader 기반.</para>
	/// </summary>
	public class DataTableWritter : Disposable
	{
		/// <summary>
		/// 프로퍼티 목록.
		/// </summary>
		public static readonly string[] Properties = new string[] { "field", "type", "option", "comment", "record" };

		/// <summary>
		/// 비 문자열 값 타입 목록.
		/// </summary>
		public static readonly string[] NoneStringMappingTypes = new string[]
		{
			"bool",
			"char",
			"sbyte",
			"byte",
			"short",
			"ushort",
			"int",
			"uint",
			"long",
			"ulong",
			"float",
			"double",
		};

		///// <summary>
		///// 문자열 값 타입 목록.
		///// </summary>
		//public static readonly string[] StringMappingTypes = new string[]
		//{
		//	"string",			
		//	"char",
		//	"String",
		//	"Char",
		//	"Guid",
		//	"DateTime",
		//	"DateTimeOffset",
		//	"Uri"
		//};

		/// <summary>
		/// 텍스트 생성기.
		/// </summary>
		private StringBuilder m_StringBuilder;

		/// <summary>
		/// 원시 테이블 목록.
		/// <para>Dictionary: { Key: {XLSXFilePath}, Value: { List: {RawTable} } }</para>
		/// </summary>
		private Dictionary<string, List<RawTable>> m_RawTables;

		/// <summary>
		/// 데이터 테이블 목록.
		/// <para>Dictionary: { Key: {TableGroupName}, Value: Dictionary: { Key: {TableName}, Value: {DataTable} } }</para>
		/// </summary>
		private Dictionary<string, Dictionary<string, DataTable>> m_DataTables;

		/// <summary>
		/// 로거.
		/// </summary>
		private Logger m_Logger;

		/// <summary>
		/// 생성됨.
		/// </summary>
		public DataTableWritter(Logger logger) : base()
		{
			m_StringBuilder = new StringBuilder();
			m_RawTables = new Dictionary<string, List<RawTable>>();
			m_DataTables = new Dictionary<string, Dictionary<string, DataTable>>();
			m_Logger = logger;
		}

		/// <summary>
		/// 해제됨.
		/// </summary>
		protected override void OnDispose(bool explicitDisposing)
		{
			m_StringBuilder.Clear();
			m_RawTables.Clear();
			m_DataTables.Clear();
		}

		/// <summary>
		/// 원시 테이블 목록 생성.
		/// </summary>
		public List<RawTable> CreateRawTablesFromXLSXFile(string excelFilePath)
		{
			if (string.IsNullOrWhiteSpace(excelFilePath) || !File.Exists(excelFilePath))
				throw new ArgumentNullException(excelFilePath);

			// 엑셀 파일 복제.
			var path = Path.GetDirectoryName(excelFilePath);
			var name = Path.GetFileNameWithoutExtension(excelFilePath);
			var extension = Path.GetExtension(excelFilePath);
			//var newExcelFilePath = Path.Combine(path, $"{name}_clone.{extension}");
			//File.Copy(excelFilePath, newExcelFilePath, false);

			// 테이블 가져오고 없으면 생성.
			if (!m_RawTables.TryGetValue(excelFilePath, out var rawTables))
			{
				rawTables = new List<RawTable>();
				m_RawTables[excelFilePath] = rawTables;
			}

			// 파일 스트림 읽기 처리.
			//using var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read);
			//var reader = ExcelReaderFactory.CreateReader(stream);

			// 파일 스트림 읽기 처리.
			//using var stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			//var reader = ExcelReaderFactory.CreateReader(stream);

			// 파일을 공유 읽기로 열어서 메모리에 적재한 뒤 메모리 스트림을 읽기 처리.
			using var fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using var memoryStream = new MemoryStream();
			fileStream.CopyTo(memoryStream);
			memoryStream.Position = 0;
			var reader = ExcelReaderFactory.CreateReader(memoryStream);

			var result = reader.AsDataSet();
			var sheets = result.Tables;
			var sheetCount = sheets.Count;
			for (var index = 0; index < sheetCount; ++index)
			{
				var sheet = sheets[index];
				if (!sheet.TableName.StartsWith("#"))
					continue;

				var rawTable = new RawTable();
				rawTable.FilePath = excelFilePath;
				rawTable.Group = name;
				rawTable.Name = sheet.TableName.TrimStart('#');

				var map = new List<string[]>();
				var rows = sheet.Rows;
				var rowCount = rows.Count;
				for (var y = 0; y < rowCount; ++y)
				{
					var columns = rows[y].ItemArray.Select(it => it?.ToString() ?? string.Empty).ToArray();
					var columnCount = columns.Length;
					if (columnCount == 0)
						continue;

					var property = columns[0].Trim().ToLower();
					if (!DataTableWritter.Properties.Contains(property))
					{
						m_Logger.Log($"[TableGenerator][{rawTable.Name}] {y}행의 맨 앞 첫번째 열에 프로퍼티가 없습니다.");
						continue;
					}

					map.Add(columns);
				}

				rawTable.Value = map.ToArray();
				rawTables.Add(rawTable);
			}

			// 복제한 파일 제거.
			//File.Delete(newExcelFilePath);
			return rawTables;
		}

		/// <summary>
		/// 데이터 테이블 생성.
		/// </summary>
		public DataTable CreateDataTableFromRawTable(RawTable rawTable)
		{
			if (rawTable == null)
				throw new ArgumentNullException(nameof(rawTable));

			var dataTable = new DataTable();
			dataTable.Group = rawTable.Group;
			dataTable.Name = rawTable.Name;

			if (!m_DataTables.TryGetValue(dataTable.Group, out var dataTables))
			{
				dataTables = new Dictionary<string, DataTable>();
				m_DataTables.Add(dataTable.Group, dataTables);
				dataTables.Add(dataTable.Name, dataTable);
			}

			var includeFields = new HashSet<int>();
			var rowCount = rawTable.Value.Length;
			for (var y = 0; y < rowCount; ++y)
			{
				var columns = rawTable.Value[y];
				var property = columns[0].Trim().ToLower();

				switch (property)
				{
					case "field":
						{
							var columnCount = columns.Length;
							for (var x = 0; x < columnCount; ++x)
							{
								var column = columns[x];
								if (x == 0 || string.IsNullOrWhiteSpace(column))
									continue;

								includeFields.Add(x);
								dataTable.Fields.Add(column);
							}
							break;
						}
					case "type":
						{
							if (dataTable.Fields.Count == 0)
								throw new Exception($"[TableGenerator][{dataTable.Name}] 타입이 필드보다 먼저 선언되었습니다.");

							var columnCount = columns.Length;
							for (var x = 0; x < columnCount; ++x)
							{
								if (!includeFields.Contains(x))
									continue;

								var column = columns[x];
								if (string.IsNullOrWhiteSpace(column))
								{
									m_Logger.Log($"[TableGenerator][{dataTable.Name}] {x}열에 타입이 비어있으므로 해당 타입은 기본값인 문자열로 간주합니다.");
									column = "string";
								}

								dataTable.Types.Add(column);
							}
							break;
						}

					case "record":
						{
							if (dataTable.Fields.Count == 0 || dataTable.Types.Count == 0)
								throw new Exception($"[TableGenerator][{dataTable.Name}] 레코드가 필드와 타입보다 먼저 선언되었습니다.");

							var record = new List<string>();
							var columnCount = columns.Length;
							for (var x = 0; x < columnCount; ++x)
							{
								if (!includeFields.Contains(x))
									continue;

								var column = columns[x];

								// 문자열 쌍따옴표(")를 특수문자(\")로 변경.
								column = column.Replace("\"", "\\\""); // "를 \\"로 변경.
								column = column.Replace("\\\\\"", "\\\""); // \\"를 \"로 변경.

								//if (string.IsNullOrWhiteSpace(column))
								//{
								//	Debug.LogWarning($"[TableGenerator][{dataTable.Name}]{x}, {y}열이 비어있으므로 타입에 기반한 기본값을 사용합니다.");
								//	column = "";
								//}

								record.Add(column);
							}

							if (record.Count > 0)
								dataTable.Records.Add(record);

							break;
						}
				}
			}

			if (dataTable.Fields.Count == 0)
				m_Logger.Log($"[TableGenerator][{dataTable.Name}] 필드 목록이 비어있습니다.");

			if (!dataTable.Fields.Contains("Id"))
				m_Logger.Log($"[TableGenerator][{dataTable.Name}] Id 필드가 없습니다.");

			if (dataTable.Types.Count == 0)
				m_Logger.Log($"[TableGenerator][{dataTable.Name}] 타입 목록이 비어있습니다.");

			if (dataTable.Records.Count == 0)
				m_Logger.Log($"[TableGenerator][{dataTable.Name}] 레코드 목록이 비어있습니다.");

			return dataTable;
		}

		///// <summary>
		///// 테이블 매니페스트 갱신.
		///// </summary>
		//public bool UpdateTableManifestFromDataTable(DataTable dataTable)
		//{

		//	if (!Directory.Exists(Project.TableDirectory))
		//		Directory.CreateDirectory(Project.TableDirectory);

		//	var manifestFilePath = $"{Project.TableDirectory}/TableManifest.result";
		//	var result = string.Empty;

		//	// 파일 불러오기.
		//	var manifest = default(TableManifest);
		//	if (File.Exists(manifestFilePath))
		//	{
		//		try
		//		{
		//			result = File.ReadAllText(manifestFilePath);
		//			manifest = JsonConvert.DeserializeObject<TableManifest>(result);
		//		}
		//		catch(Exception exception)
		//		{
		//			Debug.LogException(exception);
		//			manifest = new TableManifest();
		//		}
		//	}
		//	else
		//	{
		//		manifest = new TableManifest();
		//	}

		//	if (!manifest.TableInfos.TryGetValue(dataTable.Name, out var tableInfo))
		//		tableInfo = new TableInfo();

		//	tableInfo.Group = dataTable.Group;
		//	tableInfo.Name = dataTable.Name;
		//	tableInfo.Fields.RemoveAllPendingOperations();
		//	tableInfo.Fields.AddRange(dataTable.Fields);
		//	tableInfo.Types.RemoveAllPendingOperations();
		//	tableInfo.Types.AddRange(dataTable.Types);
		//	manifest.TableInfos[dataTable.Name] = tableInfo;

		//	// 파일 저장.
		//	result = JsonConvert.SerializeObject(tableInfo);
		//	File.WriteAllText(manifestFilePath, result);

		//	return true;
		//}

		/// <summary>
		/// CS 파일 생성.
		/// </summary>
		public void CreateCSToFile(string csFilePath, DataTable dataTable)
		{
			var parentDirectory = Path.GetDirectoryName(csFilePath);
			if (!Directory.Exists(parentDirectory))
				Directory.CreateDirectory(parentDirectory);

			var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			var fieldCount = dataTable.Fields.Count;

			m_StringBuilder.Clear();
			m_StringBuilder.AppendLine("//------------------------------------------------------------------------------");
			m_StringBuilder.AppendLine("// <auto-generated>");
			m_StringBuilder.AppendLine("// \tThis code was auto-generated by Crockhead.Table");
			m_StringBuilder.AppendLine("// \tversion 0.0.1");
			m_StringBuilder.AppendLine($"// \tupdate {timestamp}");
			m_StringBuilder.AppendLine("// </auto-generated>");
			m_StringBuilder.AppendLine("//------------------------------------------------------------------------------");
			//m_StringBuilder.AppendLine();

			m_StringBuilder.AppendLine("using Newtonsoft.Json;");
			m_StringBuilder.AppendLine("using System;");
			m_StringBuilder.AppendLine("using System.Collections.Generic;");
			m_StringBuilder.AppendLine("using Crockhead.Table;");
			m_StringBuilder.AppendLine();
			m_StringBuilder.AppendLine();

			// 클래스 시작.
			m_StringBuilder.AppendLine("/// <summary>");
			m_StringBuilder.AppendLine($"/// {dataTable.Name} Recordable Data.");
			m_StringBuilder.AppendLine("/// </summary>");
			m_StringBuilder.AppendLine("[JsonObject(MemberSerialization.OptIn)]");
			m_StringBuilder.AppendLine($"public class {dataTable.Name}Record : IRecordable");
			m_StringBuilder.AppendLine("{");

			// 멤버 작성.
			for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
			{
				var field = dataTable.Fields[fieldIndex];
				var type = dataTable.Types[fieldIndex];

				m_StringBuilder.AppendLine("\t/// <summary>");
				m_StringBuilder.AppendLine($"\t/// {field}.");
				m_StringBuilder.AppendLine("\t/// </summary>");
				m_StringBuilder.AppendLine("\t[JsonProperty]");
				m_StringBuilder.AppendLine($"\tpublic {type} {field} {{ set; get; }}");
				m_StringBuilder.AppendLine();
			}
			//m_StringBuilder.AppendLine();

			// 필드 갯수.
			m_StringBuilder.AppendLine("\t/// <summary>");
			m_StringBuilder.AppendLine("\t/// 필드 갯수.");
			m_StringBuilder.AppendLine("\t/// </summary>");
			m_StringBuilder.AppendLine("\tint IRecordable.GetFieldCount()");
			m_StringBuilder.AppendLine("\t{");
			m_StringBuilder.AppendLine($"\t\treturn {fieldCount};");
			m_StringBuilder.AppendLine("\t}");
			m_StringBuilder.AppendLine();

			// 필드 이름.
			m_StringBuilder.AppendLine("\t/// <summary>");
			m_StringBuilder.AppendLine("\t/// 필드 이름.");
			m_StringBuilder.AppendLine("\t/// </summary>");
			m_StringBuilder.AppendLine("\tstring IRecordable.GetFieldName(int index)");
			m_StringBuilder.AppendLine("\t{");
			m_StringBuilder.AppendLine("\t\tswitch (index)");
			m_StringBuilder.AppendLine("\t\t{");
			for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
			{
				var field = dataTable.Fields[fieldIndex];
				m_StringBuilder.AppendLine($"\t\t\tcase {fieldIndex}: return \"{field}\";");
			}
			m_StringBuilder.AppendLine($"\t\t\tdefault: return string.Empty;");
			m_StringBuilder.AppendLine("\t\t}");
			m_StringBuilder.AppendLine("\t}");
			m_StringBuilder.AppendLine();

			// 필드 타입.
			m_StringBuilder.AppendLine("\t/// <summary>");
			m_StringBuilder.AppendLine("\t/// 필드 타입.");
			m_StringBuilder.AppendLine("\t/// </summary>");
			m_StringBuilder.AppendLine("\tType IRecordable.GetFieldType(int index)");
			m_StringBuilder.AppendLine("\t{");
			m_StringBuilder.AppendLine("\t\tswitch (index)");
			m_StringBuilder.AppendLine("\t\t{");
			for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
			{
				var field = dataTable.Fields[fieldIndex];
				m_StringBuilder.AppendLine($"\t\t\tcase {fieldIndex}: return {field}.GetType();");
			}
			m_StringBuilder.AppendLine($"\t\t\tdefault: return null;");
			m_StringBuilder.AppendLine("\t\t}");
			m_StringBuilder.AppendLine("\t}");
			m_StringBuilder.AppendLine();

			// 필드 값.
			m_StringBuilder.AppendLine("\t/// <summary>");
			m_StringBuilder.AppendLine("\t/// 필드 값.");
			m_StringBuilder.AppendLine("\t/// </summary>");
			m_StringBuilder.AppendLine("\tobject IRecordable.GetFieldValue(int index)");
			m_StringBuilder.AppendLine("\t{");
			m_StringBuilder.AppendLine("\t\tswitch (index)");
			m_StringBuilder.AppendLine("\t\t{");
			for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
			{
				var field = dataTable.Fields[fieldIndex];
				m_StringBuilder.AppendLine($"\t\t\tcase {fieldIndex}: return {field};");
			}
			m_StringBuilder.AppendLine($"\t\t\tdefault: return null;");
			m_StringBuilder.AppendLine("\t\t}");
			m_StringBuilder.AppendLine("\t}");
			//m_StringBuilder.AppendLine();

			// 클래스 종료.
			m_StringBuilder.Append("}");

			// 저장.
			var result = m_StringBuilder.ToString();
			File.WriteAllText(csFilePath, result, new UTF8Encoding(true));
		}

		/// <summary>
		/// JSON 파일 생성.
		/// </summary>
		public void CreateJSONToFile(string jsonFilePath, DataTable dataTable)
		{
			var parentDirectory = Path.GetDirectoryName(jsonFilePath);
			if (!Directory.Exists(parentDirectory))
				Directory.CreateDirectory(parentDirectory);

			var fieldCount = dataTable.Fields.Count;
			var recordCount = dataTable.Records.Count;

			m_StringBuilder.Clear();

			m_StringBuilder.AppendLine("[");

			for (var recordIndex = 0; recordIndex < recordCount; ++recordIndex)
			{
				var record = dataTable.Records[recordIndex];

				m_StringBuilder.AppendLine("\t{");
				for (var fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
				{
					var field = dataTable.Fields[fieldIndex];
					var type = dataTable.Types[fieldIndex];
					var value = record[fieldIndex];
					var lastIndex = record.FindLastIndex(it => !string.IsNullOrWhiteSpace(it));

					// 값이 없는 유형.
					if (string.IsNullOrWhiteSpace(value))
					{
						// 아무것도 하지 않음.
						continue;
					}
					//// 문자열.
					//else if (DataTableWritter.StringMappingTypes.Contains(type))
					//{
					//	m_StringBuilder.Append($"\t\t\"{field}\": \"{value}\"");
					//}
					// 논리.
					else if (type == "bool")
					{
						value = value.ToLower();
						m_StringBuilder.Append($"\t\t\"{field}\": {value}");
					}
					// 비문자열.
					else if (DataTableWritter.NoneStringMappingTypes.Contains(type))
					{
						m_StringBuilder.Append($"\t\t\"{field}\": {value}");
					}
					// 그외.
					else
					{
						//m_StringBuilder.Append($"\t\t\"{field}\": {value}");
						m_StringBuilder.Append($"\t\t\"{field}\": \"{value}\"");
					}

					// 마지막으로 유효한 필드가 아니면 쉼표 추가.
					var lastValidField = fieldIndex == lastIndex;
					if (!lastValidField)
					{
						m_StringBuilder.Append(",");
					}
					m_StringBuilder.AppendLine();
				}

				// 마지막 레코드가 아니면 쉼표 추가.
				// ID를 넣다보니 모든 레코드는 이미 하나의 필드라도 가지고 있어서 유효함.
				m_StringBuilder.Append("\t}");
				var lastRecord = recordIndex + 1 == recordCount;
				if (!lastRecord)
				{
					m_StringBuilder.Append(",");
				}
				m_StringBuilder.AppendLine();
			}

			m_StringBuilder.Append("]");

			// 저장.
			var result = m_StringBuilder.ToString();
			File.WriteAllText(jsonFilePath, result, new UTF8Encoding(true));
		}

		/// <summary>
		/// JSON 정합성 테스트.
		/// </summary>
		public void IntegrateJSONTest(string jsonFilePath, DataTable dataTable)
		{

		}
	}
}