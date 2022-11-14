using Core.Interfaces;

namespace Core
{
	public class StringFormatter : IStringFormatter
	{
		private readonly IValidator _validator;
		private readonly IPropertyAccessCache<string> _cache;

		public static readonly StringFormatter Shared = new();

		public StringFormatter()
		{
			_validator = new StringFormaterValidator();
			_cache = new PropertyCache();
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



			return "";
		}
	}
}
