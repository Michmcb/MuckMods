namespace MuckSaveGame
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Xml.Linq;

	public static class XElementExtensions
	{
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElement(this XElement x, XName name)
		{
			XElement e = new(name);
			x.Add(e);
			return e;
		}
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// Also assigns <paramref name="value"/> to <see cref="XElement.Value"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <param name="value">The value to set on the new <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElementValue(this XElement x, XName name, string value)
		{
			XElement e = new(name, value);
			x.Add(e);
			return e;
		}
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// Also assigns <paramref name="value"/> to <see cref="XElement.Value"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <param name="value">The value to set on the new <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElementValue(this XElement x, XName name, int value)
		{
			XElement e = new(name, value.ToString(CultureInfo.InvariantCulture));
			x.Add(e);
			return e;
		}
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// Also assigns <paramref name="value"/> to <see cref="XElement.Value"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <param name="value">The value to set on the new <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElementValue(this XElement x, XName name, bool value)
		{
			XElement e = new(name, value.ToString());
			x.Add(e);
			return e;
		}
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// Also assigns <paramref name="value"/> to <see cref="XElement.Value"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <param name="value">The value to set on the new <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElementValue(this XElement x, XName name, float value)
		{
			XElement e = new(name, value.ToString(CultureInfo.InvariantCulture));
			x.Add(e);
			return e;
		}
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// Also assigns <paramref name="value"/> to <see cref="XElement.Value"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <param name="value">The value to set on the new <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElementContent(this XElement x, XName name, object content)
		{
			XElement e = new(name, content);
			x.Add(e);
			return e;
		}
		/// <summary>
		/// Adds a new <see cref="XElement"/> with the provided <paramref name="name"/>, and returns it.
		/// For every element in <paramref name="values"/>, creates another <see cref="XElement"/> with the name <paramref name="listElementName"/>
		/// as a child of the new <see cref="XElement"/>, and invokes <paramref name="setValue"/> to set the value.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name for the new <see cref="XElement"/>.</param>
		/// <param name="listElementName">The name for every child <see cref="XElement"/>.</param>
		/// <param name="values">The values.</param>
		/// <param name="setValue">The callback to set the value of each <see cref="XElement"/>.</param>
		/// <returns>The added <see cref="XElement"/>.</returns>
		public static XElement AddElementValues<T>(this XElement x, XName name, XName listElementName, IEnumerable<T> values, Action<XElement, T> setValue)
		{
			XElement e = x.AddElement(name);
			foreach (T v in values)
			{
				setValue(e.AddElement(listElementName), v);
			}
			return e;
		}
		/// <summary>
		/// Attempts to retrieve an <see cref="XElement"/> with the provided <paramref name="name"/>.
		/// If it is not present, throws an <see cref="InvalidDataException"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <param name="name">The name of the element to retrieve.</param>
		/// <returns>The <see cref="XElement"/> with the requested name.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static XElement RequiredElement(this XElement x, XName name)
		{
			return x.Element(name) ?? throw new InvalidDataException(string.Concat("Element \"", x.Name.ToString(), "\" is missing the required element \"", name.ToString(), "\""));
		}
		/// <summary>
		/// Attempts to parse the <see cref="XElement.Value"/> as an <see cref="int"/>.
		/// If it cannot be parsed as an <see cref="int"/>, throws an <see cref="InvalidDataException"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <returns>The parsed value.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static int ValueAsInt(this XElement x)
		{
			return int.TryParse(x.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v)
				? v
				: throw new InvalidDataException(string.Concat("Unable to parse the value \"", x.Value, "\" of element \"", x.Name.ToString(), "\" as an integer"));
		}
		/// <summary>
		/// Attempts to parse the <see cref="XElement.Value"/> as a <see cref="float"/>.
		/// If it cannot be parsed as a <see cref="float"/>, throws an <see cref="InvalidDataException"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <returns>The parsed value.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static float ValueAsFloat(this XElement x)
		{
			return float.TryParse(x.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float v)
				? v
				: throw new InvalidDataException(string.Concat("Unable to parse the value \"", x.Value, "\" of element \"", x.Name.ToString(), "\" as a floating-point number"));
		}
		/// <summary>
		/// Attempts to parse the <see cref="XElement.Value"/> as a <see cref="bool"/>.
		/// If it cannot be parsed as a <see cref="bool"/>, throws an <see cref="InvalidDataException"/>.
		/// </summary>
		/// <param name="x">The <see cref="XElement"/>.</param>
		/// <returns>The parsed value.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static bool ValueAsBool(this XElement x)
		{
			return bool.TryParse(x.Value, out bool v)
				? v
				: throw new InvalidDataException(string.Concat("Unable to parse the value \"", x.Value, "\" of element \"", x.Name.ToString(), "\" as a boolean"));
		}
	}
}
