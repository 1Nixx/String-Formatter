using System.Collections.Concurrent;
using System.Linq.Expressions;


namespace Core.Interfaces
{
	internal interface IPropertyAccessCache<T> where T: class
	{
		T GetCached(object target, string propOrFieldName);
		bool TryCache(Type target);
	}
}
