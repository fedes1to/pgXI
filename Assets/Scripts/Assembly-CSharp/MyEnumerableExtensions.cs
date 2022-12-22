using System;
using System.Collections.Generic;
using System.Linq;

public static class MyEnumerableExtensions
{
	public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
	{
		Random rnd = new Random();
		return source.OrderBy((T item) => rnd.Next());
	}
}
