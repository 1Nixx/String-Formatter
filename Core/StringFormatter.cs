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

			var result = new StringBuilder();

			bool isData = false, isComment = false;

			var workData = new StringBuilder();

			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == '{')
				{
					if (!isData)
					{
						isData = true;
						workData.Clear();

					}
					else
					{
						isData = false;
						isComment = true;
						result.Append(input[i]);
					}
				}

				if (input[i] == '}')
				{
					if (isData)
					{
						isData = false;
						result.Append(_cache.GetCached(target, workData.ToString()));
					}

					if (isComment)
					{
						isComment = false;
						result.Append(input[i]);
					}

				}

				if (input[i] != '}' && input[i] != '{')
				{
					if (!isData)
					{
						result.Append(input[i]);
					}

					{
						workData.Append(input[i]);
					}
				}
			}

			return result.ToString();
		}
	}
}
