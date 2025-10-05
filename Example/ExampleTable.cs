using Crockhead.Core;
using Crockhead.Table;


/// <summary>
/// 샘플 테이블.
/// </summary>
[FilePath("Assets/Resources/Table/ExampleTable.json")]
public class ExampleTable : SharedTable<ExampleTable, ExampleTableRecord, RecordArrayFileReader<ExampleTableRecord>>
{
	/// <summary>
	/// 생성.
	/// </summary>
	protected override void OnCreate(params object[] arguments)
	{
		base.OnCreate(arguments);

		// 파일로부터 레코드 배열을 읽어오는 리더.
		var reader = new RecordArrayFileReader<ExampleTableRecord>(this);
		//SetReader(reader);
		Load(reader);
	}

	/// <summary>
	/// 해제됨.
	/// </summary>
	protected override void OnDispose(bool explicitDisposing)
	{
		base.OnDispose(explicitDisposing);
	}
}