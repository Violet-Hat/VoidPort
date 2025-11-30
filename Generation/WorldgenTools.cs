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
		
		/*
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
		*/
	}
}