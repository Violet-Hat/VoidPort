using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.ModLoader;

namespace VoidPort.Common
{
	public class WorldgenTools
	{	
		public static Vector2 cuadraticBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2)
		{
			float a = 1 - t;
			float aa = a * a;
            float tt = t * t;
			
            Vector2 p = aa * p0;
            p += 2 * a * t * p1;
            p += tt * p2;
            return p;
		}
		
		public static Vector2 cubicBezier(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			float a = 1 - t;
			float aa = a * a;
			float aaa = aa * a;
            float tt = t * t;
			float ttt = tt * t;

            Vector2 p = aaa * p0;
            p += 3 * aa * t * p1;
			p += 3 * a * tt * p2;
			p += ttt * p3;
            return p;
		}

		static public int MooreTiles(int x, int y)
        {
            int count = 0;

            for (int nebX = x - 1; nebX <= x + 1; nebX++)
            {
                for (int nebY = y - 1; nebY <= y + 1; nebY++)
                {
                    if (nebX != x || nebY != y)
                    {
                        if (Main.tile[nebX, nebY].HasTile)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

		static public int NeumanTiles(int x, int y)
        {
            int count = 0;

            if (Main.tile[x, y - 1].HasTile) count++;
			if (Main.tile[x - 1, y].HasTile) count++;
			if (Main.tile[x + 1, y].HasTile) count++;
			if (Main.tile[x, y + 1].HasTile) count++;

            return count;
        }

		#region "Perlin noise simple"
		private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

		private static float Lerp(float a, float b, float t) => a + t * (b - a);

		private static float Grad(int p) => (p & 1) == 0 ? 1f : -1f;

		private static uint StableHash(int x)
		{
			uint u = (uint)x;
			u = ((u >> 16) ^ u) * 0x45d9f3b;
			u = ((u >> 16) ^ u) * 0x45d9f3b;
			u = (u >> 16) ^ u;
			return u;
		}

		private static float Perlin1D(float x, int seed)
		{
			int xi = (int)MathF.Floor(x);
			float xf = x - xi;

			int h0 = (int)StableHash(xi + seed);
			int h1 = (int)StableHash(xi + 1 + seed);

			float d0 = Grad(h0) * xf;
			float d1 = Grad(h1) * (xf - 1);

			float u = Fade(xf);

			return Lerp(d0, d1, u);
		}

		public static float Perlin(float x, int seed, int octaves = 4, float persistence = 0.5f)
		{
			float total = 0f;
			float frequency = 1f;
			float amplitude = 1f;
			float maxValue = 0f;

			for (int i = 0; i < octaves; i++)
			{
				total += Perlin1D(x * frequency, seed + i) * amplitude;
				maxValue += amplitude;

				amplitude *= persistence;
				frequency *= 2f;
			}

			return (total / maxValue) * 0.5f + 0.5f;
		}
		#endregion

		#region "Perlin noise 2D"
		internal static readonly List<Vector2> Directions = new List<Vector2>()
		{
			new Vector2(-1f, -1f),
			new Vector2(1f, -1f),
			new Vector2(-1f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, -1f),
			new Vector2(-1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 0f),
		};
		
		public static float PerlinNoise2D(float x, float y, int octaves, int seed)
		{
			float SmoothFunction(float n) => 3f * n * n - 2f * n * n * n;

			float NoiseGradient(int s, int noiseX, int noiseY, float xd, float yd)
			{
				int hash = s;
				hash ^= 1619 * noiseX;
				hash ^= 31337 * noiseY;

				hash = hash * hash * hash * 60493;
				hash = (hash >> 13) ^ hash;

				Vector2 g = Directions[hash & 7];

				return xd * g.X + yd * g.Y;
			}

			int frequency = (int)Math.Pow(2D, octaves);
			x *= frequency;
			y *= frequency;

			int flooredX = (int)x;
			int flooredY = (int)y;
			int ceilingX = flooredX + 1;
			int ceilingY = flooredY + 1;
			float interpolatedX = x - flooredX;
			float interpolatedY = y - flooredY;
			float interpolatedX2 = interpolatedX - 1;
			float interpolatedY2 = interpolatedY - 1;

			float fadeX = SmoothFunction(interpolatedX);
			float fadeY = SmoothFunction(interpolatedY);

			float smoothX = MathHelper.Lerp(NoiseGradient(seed, flooredX, flooredY, interpolatedX, interpolatedY), NoiseGradient(seed, ceilingX, flooredY, interpolatedX2, interpolatedY), fadeX);
			float smoothY = MathHelper.Lerp(NoiseGradient(seed, flooredX, ceilingY, interpolatedX, interpolatedY2), NoiseGradient(seed, ceilingX, ceilingY, interpolatedX2, interpolatedY2), fadeX);

			return MathHelper.Lerp(smoothX, smoothY, fadeY);
		}
		#endregion
	}
}