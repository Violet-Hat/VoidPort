using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using static Terraria.WorldGen;
using static tModPorter.ProgressUpdate;

using VoidPort.Common;
using VoidPort.Tiles.Void;
using VoidPort.NPCs.Boss.Zero;

namespace VoidPort.Generation
{
	public class VoidGen : ModSystem
	{
		//Biome generation
		private void VoidIslands(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Obstructing the skies";
			
			List<Point> IslandPositions = new List<Point>();
			
			int PlaceBiomeX = Main.maxTilesX - (Main.maxTilesX / 15);
			int PlaceBiomeY = 120;
			
			Point VoidOrigin = new Point(PlaceBiomeX, PlaceBiomeY);

			if (Main.maxTilesX > 4200)
			{
				IslandPositions.Add(getIslandPoint(VoidOrigin.X + 70, VoidOrigin.Y + 40));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X + 70, VoidOrigin.Y - 40));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X + 170, VoidOrigin.Y));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X - 70, VoidOrigin.Y + 40));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X - 70, VoidOrigin.Y - 40));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X - 170, VoidOrigin.Y));
			}
			else
			{
				IslandPositions.Add(getIslandPoint(VoidOrigin.X, VoidOrigin.Y + 25, true, false));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X + 35, VoidOrigin.Y + 25, true, false));
				IslandPositions.Add(getIslandPoint(VoidOrigin.X - 35, VoidOrigin.Y + 25, true, false));
			}
			
			foreach(var Position in IslandPositions)
			{
				int width = WorldGen.genRand.Next(50, 61);
				int height = (int)(width * 0.55f);

				GenerateIsland(Position.X, Position.Y, width, height);

				//Create cracks
				Runner(Position.X, Position.Y, (int)(width * 0.55f), (int)(height * 0.55f), WorldGen.genRand.Next(0, 6), 3.75f, 80, 101, -1);

				//Clean up
				CellularAutomata(Position.X, Position.Y, width, height);

				//Place soft dirt surface
				SoftSurface(Position.X, Position.Y, width, height);

				//Place apocalyptite
				Runner(Position.X, Position.Y, (int)(width * 0.65f), (int)(height * 0.65f), WorldGen.genRand.Next(3, 6), 5.25f, 15, 25, ModContent.TileType<Apocalyptite>());
			}
			
			//Spawn Zero aka Strange Machine
			/*
			Flags.whereIsZero = new Vector2(VoidOrigin.X * 16, VoidOrigin.Y * 16);
			int strangeMachine = NPC.NewNPC(null, (int)Flags.whereIsZero.X, (int)Flags.whereIsZero.Y, ModContent.NPCType<ZeroDeactivated>());
			Main.npc[strangeMachine].position.X -= 7;
			Main.npc[strangeMachine].position.Y -= 4;
			*/
		}

		//Generate islands
		public static void GenerateIsland(int cx, int cy, int width, int height)
		{
			int seed = WorldGen.genRand.Next();

			float sharpness = WorldGen.genRand.NextFloat(1.5f, 1.9f);
			float depthMult = WorldGen.genRand.NextFloat(1.6f, 1.9f);
			float bottomFreq = WorldGen.genRand.NextFloat(0.05f, 0.09f);

			for (int x = cx - width; x <= cx + width; x++)
			{
				float dx = MathF.Abs(x - cx);

				float normalized = dx / width;
				if (normalized > 1f) continue;

				//Top
				float topMask = MathF.Sqrt(1f - MathF.Pow(normalized, 2f)); //Round
				float topNoise = WorldgenTools.Perlin(x * 0.04f, seed, 3, 0.4f);
				int topOffset = (int)(topMask * (height * 0.3f) * topNoise);

				//Bottom
				float bottomMask = MathF.Pow(1f - normalized, sharpness); //Triangular
				float bottomNoise = WorldgenTools.Perlin(x * bottomFreq, unchecked(seed - 1), 5, 0.6f);
				int bottomOffset = (int)(bottomMask * height * depthMult * bottomNoise);
				
				for (int y = cy - topOffset; y < cy + bottomOffset; y++)
				{
					WorldGen.PlaceTile(x, y, ModContent.TileType<Doomstone>(), true);
					WorldGen.PlaceWall(x, y, ModContent.WallType<DoomstoneWall>(), true);
				}
			}

			//Remove walls touching air
			for (int x = cx - width - 2; x <= cx + width + 2; x++)
			{
				for (int y = cy - (int)(height / 3); y < cy + height + 2; y++)
				{
					int tilesAround = WorldgenTools.MooreTiles(x, y);

					if (tilesAround < 8)
					{
						WorldGen.KillWall(x, y);
					}
				}
			}
		}

		//Get points for the islands
		public static Point getIslandPoint(int i, int j, bool randX = true, bool randY = true)
		{
			int offsetX = WorldGen.genRand.Next(-5, 6);
			int offsetY = WorldGen.genRand.Next(-5, 6);

			int x = i;
			int y = j;
			
			if(randX)
				x += offsetX;

			if(randY)
				y += offsetY;
			
			return new Point(x, y);
		}

		//Tile runner (scary)
		public static void Runner(int x, int y, int width, int height, int amount, float strength, int rangeMin, int rangeMax, int type)
        {
			int numOres = amount;
			int i = 0;
			
			while(numOres > 0 && i++ < 100000)
			{
				int offSetX = WorldGen.genRand.Next(-width, width + 1);
				int offSetY = WorldGen.genRand.Next(height + 1);
				
				if(Main.tile[x, y].HasTile)
				{
					numOres--;
					WorldGen.TileRunner(x + offSetX, y + offSetY, strength, WorldGen.genRand.Next(rangeMin, rangeMax), type);
				}
			}
		}

		//Soft surface
		public static void SoftSurface(int cx, int cy, int width, int height)
		{
			for (int x = cx - width - 2; x <= cx + width + 2; x++)
			{
				int softCount = 0;

				for (int y = cy - (int)(height / 3); y < cy + height + 2; y++)
				{
					Tile tile = Main.tile[x, y];

					if (tile.HasTile && WorldGen.SolidTile(x, y) && softCount++ < 5)
					{
						tile.TileType = (ushort)ModContent.TileType<SoftDoomstone>();
					}
					else if(!tile.HasTile && tile.WallType > WallID.None)
					{
						softCount++;
					}
				}
			}
		}

		//Clean up
		public static void CellularAutomata(int cx, int cy, int width, int height)
		{
			for (int l = 0; l < 1; l++)
			{
				for (int x = cx - width - 2; x <= cx + width + 2; x++)
				{
					for (int y = cy - (height / 3); y < cy + height + 2; y++)
					{
						Tile tile = Main.tile[x, y];

						if (tile.WallType > WallID.None)
						{
							int tilesAround = WorldgenTools.MooreTiles(x, y);

							if (tilesAround < 4)
							{
								WorldGen.KillTile(x, y, noItem: true);
							}
							else if(tilesAround > 4)
							{
								WorldGen.PlaceTile(x, y, ModContent.TileType<Doomstone>(), true);
							}
						}
					}
				}
			}
		}

		//Add the biome in the worldgen task
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int BiomesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if(BiomesIndex != -1)
			{
				tasks.Insert(BiomesIndex + 1, new PassLegacy("Void Islands", VoidIslands));
			}
		}
	}
}