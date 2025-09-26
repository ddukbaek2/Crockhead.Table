using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
//using UnityEngine;


namespace Crockhead.Table
{
	/// <summary>
	/// 테이블 필드로 사용 가능한 딕셔너리.
	/// </summary>
	public class JsonDictionary<TDictionary, TKey, TValue> : Dictionary<TKey, TValue>
		where TDictionary : JsonDictionary<TDictionary, TKey, TValue>, new()
	{
		/// <summary>
		/// 변환 처리기.
		/// </summary>
		public class Converter : JsonConverter<TDictionary>
		{
			/// <summary>
			/// 읽기.
			/// </summary>
			public override TDictionary ReadJson(JsonReader reader, Type objectType, TDictionary existingValue,
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

					var dictionary = token.ToObject<Dictionary<TKey, TValue>>(serializer);
					var result = hasExistingValue && existingValue != null ? existingValue : new TDictionary();
					result.Clear();
					foreach (var pair in dictionary)
					{
						result[pair.Key] = pair.Value;
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
			public override void WriteJson(JsonWriter writer, TDictionary value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, (Dictionary<TKey, TValue>)value);
			}
		}
	}


	/// <summary>
	/// 논리 사전 배열.
	/// </summary>
	[JsonConverter(typeof(BoolDictionary.Converter))]
	public class BoolDictionary : JsonDictionary<BoolDictionary, string, bool> { }


	/// <summary>
	/// 문자열 사전 배열.
	/// </summary>
	[JsonConverter(typeof(StringDictionary.Converter))]
	public class StringDictionary : JsonDictionary<StringDictionary, string, string> { }


	/// <summary>
	/// 정수 사전 배열.
	/// </summary>
	[JsonConverter(typeof(ShortDictionary.Converter))]
	public class ShortDictionary : JsonDictionary<ShortDictionary, string, short> { }


	/// <summary>
	/// 정수 사전 배열.
	/// </summary>
	[JsonConverter(typeof(IntDictionary.Converter))]
	public class IntDictionary : JsonDictionary<IntDictionary, string, int> { }


	/// <summary>
	/// 정수 사전 배열.
	/// </summary>
	[JsonConverter(typeof(LongDictionary.Converter))]
	public class LongDictionary : JsonDictionary<LongDictionary, string, long> { }


	/// <summary>
	/// 실수 사전 배열.
	/// </summary>
	[JsonConverter(typeof(FloatDictionary.Converter))]
	public class FloatDictionary : JsonDictionary<FloatDictionary, string, float> { }


	/// <summary>
	/// 실수 사전 배열.
	/// </summary>
	[JsonConverter(typeof(DoubleDictionary.Converter))]
	public class DoubleDictionary : JsonDictionary<DoubleDictionary, string, double> { }
}