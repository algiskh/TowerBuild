using System;

public static class Extensions
{
	public static bool HasAll<T>(this T self, T flags)
		where T : Enum
	{
		long selfValue = Convert.ToInt64(self);
		long flagsValue = Convert.ToInt64(flags);
		return (selfValue & flagsValue) == flagsValue;
	}

	public static bool HasAnyCommon<T>(this T a, T b)
		where T : Enum
	{
		long aValue = Convert.ToInt64(a);
		long bValue = Convert.ToInt64(b);
		return (aValue & bValue) != 0;
	}
}
