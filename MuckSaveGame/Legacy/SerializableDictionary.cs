namespace MuckSaveGame.Legacy
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;

	[Serializable]
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		private const string DefaultItemTag = "item";
		private const string DefaultKeyTag = "key";
		private const string DefaultValueTag = "value";

		readonly XmlSerializer keySerializer = new(typeof(TKey));
		readonly XmlSerializer valueSerializer = new(typeof(TValue));

		public SerializableDictionary()
		{
		}

		protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		protected virtual string ItemTagName => DefaultItemTag;

		protected virtual string KeyTagName => DefaultKeyTag;

		protected virtual string ValueTagName => DefaultValueTag;

		public XmlSchema GetSchema()
		{
			return null!;
		}

		public void ReadXml(XmlReader reader)
		{
			var wasEmpty = reader.IsEmptyElement;

			reader.Read();
			if (wasEmpty)
			{
				return;
			}

			try
			{
				while (reader.NodeType != XmlNodeType.EndElement)
				{
					ReadItem(reader);
					reader.MoveToContent();
				}
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			foreach (var keyValuePair in this)
			{
				WriteItem(writer, keyValuePair);
			}
		}

		private void ReadItem(XmlReader reader)
		{
			reader.ReadStartElement(ItemTagName);
			try
			{
				Add(ReadKey(reader), ReadValue(reader));
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		private TKey ReadKey(XmlReader reader)
		{
			reader.ReadStartElement(KeyTagName);
			try
			{
				return (TKey)keySerializer.Deserialize(reader);
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		private TValue ReadValue(XmlReader reader)
		{
			reader.ReadStartElement(ValueTagName);
			try
			{
				return (TValue)valueSerializer.Deserialize(reader);
			}
			finally
			{
				reader.ReadEndElement();
			}
		}

		private void WriteItem(XmlWriter writer, KeyValuePair<TKey, TValue> keyValuePair)
		{
			writer.WriteStartElement(ItemTagName);
			try
			{
				WriteKey(writer, keyValuePair.Key);
				WriteValue(writer, keyValuePair.Value);
			}
			finally
			{
				writer.WriteEndElement();
			}
		}

		private void WriteKey(XmlWriter writer, TKey key)
		{
			writer.WriteStartElement(KeyTagName);
			try
			{
				keySerializer.Serialize(writer, key);
			}
			finally
			{
				writer.WriteEndElement();
			}
		}

		private void WriteValue(XmlWriter writer, TValue value)
		{
			writer.WriteStartElement(ValueTagName);
			try
			{
				valueSerializer.Serialize(writer, value);
			}
			finally
			{
				writer.WriteEndElement();
			}
		}
	}
}