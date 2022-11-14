using Core.Interfaces;


namespace Core
{
	internal class StringFormaterValidator : IValidator<string>
	{
		public bool IsValid(string input)
		{
			int sum = 0;

			for (int i = 0; i < input.Length; i++)
			{

				if (sum > 2 || sum < 0 ||
					(sum == 2 && input[i] == '}' && (i + 1 >= input.Length || input[i + 1] != '}')) ||
					(sum == 1 && input[i] == '{' && input[i - 1] != '{'))
				{
					return false;
				}

				if (input[i] == '{')
				{
					sum++;
				}

				if (input[i] == '}')
				{

					sum--;
				}

			}

			if (sum != 0)
			{
				return false;
			}

			return true;
		}
	}
}
