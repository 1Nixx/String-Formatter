using Core.Interfaces;

namespace Core
{
	public class StringFormaterValidator : IValidator<string>
	{
		public bool IsValid(string input)
		{
			int singleBraceCounter = 0;
			int doubleBraceCounter = 0;

			ParceState state = ParceState.Entry;
			for (int i = 0; i < input.Length; i++)
			{
				switch (state)
				{
					case ParceState.Entry:
						if (input[i] == '{')
							state = ParceState.LeftBrace;
						else if (input[i] != '}')
							state = ParceState.AnyText;
						else
							return false;
						break;
					case ParceState.LeftBrace:
						singleBraceCounter++;

						if (input[i] == '{')
							state = ParceState.LeftDoubleBrace;
						else if (input[i] == '}')
							return false;
						else if (IsValidIdentificatorChar(input[i]))
							state = ParceState.Identificator;
						else
							return false;
						break;
					case ParceState.RightBrace:
						singleBraceCounter--;

						if (input[i] == '}')
							state = ParceState.RightDoubleBrace;
						else if (input[i] == '{')
							state = ParceState.LeftBrace;
						else 
							state = ParceState.AnyText;
						break;
					case ParceState.LeftDoubleBrace:
						singleBraceCounter--;
						doubleBraceCounter++;

						if (input[i] == '{')
							state = ParceState.LeftBrace;
						else if (input[i] == '}')
							state = ParceState.RightBrace;
						else
							state = ParceState.AnyText;
						break;
					case ParceState.RightDoubleBrace:
						singleBraceCounter++;
						doubleBraceCounter--;

						if (input[i] == '{')
							state = ParceState.LeftBrace;
						else if (input[i] == '}')
							state = ParceState.RightBrace;
						else
							state = ParceState.AnyText;
						break;
					case ParceState.AnyText:
						if (singleBraceCounter != 0)
							return false;

						if (input[i] == '{')
							state = ParceState.LeftBrace;
						else if (input[i] == '}')
							state = ParceState.RightBrace;
						else
							state = ParceState.AnyText;
						break;
					case ParceState.Identificator:
						if (singleBraceCounter != 1)
							return false;

						if (input[i] == '}')
							state = ParceState.RightBrace;
						else if (IsValidIdentificatorChar(input[i]))
							state = ParceState.Identificator;
						else
							return false;
						break;
					default:
						return false;
				}
			}

			if (state == ParceState.RightBrace)
				singleBraceCounter--;
			else if (state == ParceState.RightDoubleBrace)
				singleBraceCounter++;

			return singleBraceCounter == 0;
		}

		private bool IsValidIdentificatorChar(char item)
		{
			return "QWERTYUIOPASDFGHJKLZXCVBNM".Contains(char.ToUpper(item));
		}

		private enum ParceState
		{
			Entry,
			LeftBrace,
			RightBrace,
			LeftDoubleBrace,
			RightDoubleBrace,
			AnyText,
			Identificator
		}
	}
}
