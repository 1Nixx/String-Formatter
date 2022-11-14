using Core.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Core
{
	internal class EntityCache : IPropertyAccessCache<string>
	{
		private ConcurrentDictionary<string, Expression<Func<object, string>>> _cache = new();

		public string GetCached(object target, string propOrFieldName)
		{
			string cacheKey = GetCacheKey(target.GetType(), propOrFieldName);

			if (!_cache.ContainsKey(cacheKey))
				CachePropOrField(target.GetType(), propOrFieldName);

			var func = _cache[cacheKey];
			return func.Compile()(target);
		}

		public bool TryCache(Type target)
		{
			var propList = target.GetProperties().Select(p => p.Name);
			var fieldList = target.GetFields().Select(p => p.Name);

			var propsAndFields = propList.Concat(fieldList).ToList();

			foreach (var item in propsAndFields)
			{
				CachePropOrField(target, item);
			}

			return true;
		}

		private void CachePropOrField(Type target, string propOrFieldName)
		{
			string cacheKey = GetCacheKey(target, propOrFieldName);

			ParameterExpression generalObjParam = Expression.Parameter(typeof(object), "obj");

			var curObjParam = Expression.PropertyOrField(Expression.TypeAs(generalObjParam, target), propOrFieldName);

			var b = Expression.Call(curObjParam, "ToString", null, null);

			Expression<Func<object, string>> exp =
				Expression.Lambda<Func<object, string>>(b, new ParameterExpression[] { generalObjParam });

			_cache.TryAdd(cacheKey, exp);
		}

		private string GetCacheKey(Type target, string propOrFieldName)
		{
			return target.ToString() + "." + propOrFieldName;
		}
	}
}
