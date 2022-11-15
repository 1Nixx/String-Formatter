using Core.Interfaces;
using System.Text;

namespace Core
{
	public class StringFormatter : IStringFormatter
	{
		private readonly IValidator<string> _validator;
		private readonly IPropertyAccessCache<string> _cache;

		public static readonly StringFormatter Shared = new();

		public StringFormatter()
		{
			_validator = new StringFormaterValidator();
			_cache = new EntityCache();
		}

		public string Format(string template, object target)
		{
			if (!_validator.IsValid(template))
				throw new FormatException("Invalid template format");

			_cache.TryCache(target.GetType());
			
			return ParceString(template, target);
		}

		private string ParceString(string input, object target)
		{
			int i = -1;

			int leftBorder, rightBorder;
			var stringBuilder = new StringBuilder();

			while (i < input.Length)
			{
				leftBorder = IndexOfAloneSymbol(input, '{', i + 1);
				if (leftBorder == -1)
				{
					stringBuilder.Append(input.Substring(i + 1)).Replace("{{", "{").Replace("}}", "}");
					break;
				}
				else
				{
					stringBuilder.Append(input.Substring(i + 1, leftBorder - i - 1)).Replace("{{", "{").Replace("}}", "}");
					i = leftBorder;

					rightBorder = IndexOfAloneSymbol(input, '}', i + 1);
					i = rightBorder;

					stringBuilder.Append(_cache.GetCached(target, input.Substring(leftBorder + 1, rightBorder - leftBorder - 1)));
				}
			}

			return stringBuilder.ToString();
		}

		private int IndexOfAloneSymbol(string input, char symbol, int startIndex)
		{
			int i = startIndex;
			int pos = -1;

			while (i < input.Length)
			{
				i = input.IndexOf(symbol, i);

				if (i == -1)
					break;

				if (i == input.Length - 1)
				{
					pos = i;
					break;
				}

				if (i < input.Length - 1)
					if (input[i + 1] != symbol)
					{
						pos = i;
						break;
					}

				i += 2;
			}
			
			return pos;
		}
	}
}
