using System.Collections.Generic;

namespace Rilisoft
{
	public sealed class TypeModeGameComparer : IEqualityComparer<TypeModeGame>
	{
		private static readonly TypeModeGameComparer s_instance = new TypeModeGameComparer();

		public static TypeModeGameComparer Instance
		{
			get
			{
				return s_instance;
			}
		}

		private TypeModeGameComparer()
		{
		}

		public bool Equals(TypeModeGame x, TypeModeGame y)
		{
			return x == y;
		}

		public int GetHashCode(TypeModeGame obj)
		{
			return (int)obj;
		}
	}
}
