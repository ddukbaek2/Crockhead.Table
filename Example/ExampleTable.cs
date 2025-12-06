using Crockhead.Core;
using Crockhead.Table;


/// <summary>
/// 샘플 테이블.
/// </summary>
[FilePath("Assets/Resources/Table/ExampleTable.json")]
public class ExampleTable : SharedTable<ExampleTable, ExampleTableRecord>
{
	/// <summary>
	/// 생성.
	/// </summary>
	protected override void OnCreate()
	{
		base.OnCreate();

		// 파일로부터 레코드 배열을 읽어오는 리더.
		var reader = new RecordArrayFileReader<ExampleTableRecord>(this);
		LoadTable(reader);
	}

	/// <summary>
	/// 해제됨.
	/// </summary>
	protected override void OnDispose()
	{
		base.OnDispose();
	}
}