using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlazmaGames.Core.Utils
{
	public static class Iter
	{
		public static void ForEach<T>(IEnumerable<T> itbl, Action<T> func)
		{
			foreach (T t in itbl) func(t);
		}

		public static void ForEach<T>(IEnumerable<T> itbl, Action<T, int> func)
		{
			int i = 0;
			foreach (T t in itbl) func(t, i++);
		}

        public static void Loop2D(Vector2Int range, Func<Vector2Int, bool> func)
        {
            for (int y = 0; y < range.y; y++)
            {
                for (int x = 0; x < range.x; x++)
                {
                    if (!func(new Vector2Int(x, y))) return;
                }
            }
        }

        public static void Loop3D(Vector3Int range, Func<Vector3Int, bool> func)
		{
			for (int z = 0; z < range.z; z++)
			{
				for (int y = 0; y < range.y; y++)
				{
					for (int x = 0; x < range.x; x++)
					{
						if (!func(new Vector3Int(x, y, z))) return;
					}
				}
			}
		}

		public static void Loop3D(Vector3Int range, Vector3Int stride, Func<Vector3Int, bool> func)
		{
			Vector3Int vec = new Vector3Int();
			for (int z = 0; z < range.z; z += stride.z)
			{
				for (int y = 0; y < range.y; y += stride.y)
				{
					for (int x = 0; x < range.x; x += stride.x)
					{
						vec.Set(x, y, z);
						if (!func(vec)) return;
					}
				}
			}
		}
	}
}
