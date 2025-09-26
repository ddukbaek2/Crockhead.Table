using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using UnityEngine;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블 필드로 사용 가능한 리스트.
	/// </summary>
	public class JsonList<TList, TValue> : List<TValue>
		where TList : JsonList<TList, TValue>, new()
	{
		/// <summary>
		/// 변환 처리기.
		/// </summary>
		public class Converter : JsonConverter<TList>
		{
			/// <summary>
			/// 읽기.
			/// </summary>
			public override TList ReadJson(JsonReader reader, Type objectType, TList existingValue,
				bool hasExistingValue, JsonSerializer serializer)
			{
				var json = reader.Value.ToString();
				try
				{
					if (reader.TokenType == JsonToken.Null)
						return null;

					var token = JToken.ReadFrom(reader);
					if (token.Type == JTokenType.String)
						token = JToken.Parse(token.Value<string>());

					var list = token.ToObject<List<TValue>>(serializer);
					var result = hasExistingValue && existingValue != null ? existingValue : new TList();
					result.Clear();
					foreach (var item in list)
					{
						result.Add(item);
					}

					return result;
				}
				catch// (Exception exception)
				{
					//Debug.LogException(exception);
					throw;
				}
			}

			/// <summary>
			/// 쓰기.
			/// </summary>
			public override void WriteJson(JsonWriter writer, TList value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, (List<TValue>)value);
			}
		}
	}


	/// <summary>
	/// 논리 목록.
	/// </summary>
	[JsonConverter(typeof(BoolList.Converter))]
	public class BoolList : JsonList<BoolList, bool> { }


	/// <summary>
	/// 문자열 목록.
	/// </summary>
	[JsonConverter(typeof(StringList.Converter))]
	public class StringList : JsonList<StringList, string> { }

	/// <summary>
	/// 정수 목록.
	/// </summary>
	[JsonConverter(typeof(ShortList.Converter))]
	public class ShortList : JsonList<ShortList, short> { }


	/// <summary>
	/// 정수 목록.
	/// </summary>
	[JsonConverter(typeof(IntList.Converter))]
	public class IntList : JsonList<IntList, int> { }


	/// <summary>
	/// 정수 목록.
	/// </summary>
	[JsonConverter(typeof(LongList.Converter))]
	public class LongList : JsonList<LongList, long> { }


	/// <summary>
	/// 실수 목록.
	/// </summary>
	[JsonConverter(typeof(FloatList.Converter))]
	public class FloatList : JsonList<FloatList, float> { }


	/// <summary>
	/// 실수 목록.
	/// </summary>
	[JsonConverter(typeof(DoubleList.Converter))]
	public class DoubleList : JsonList<DoubleList, double> { }
}