using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.WorldBuilding;
using Terraria.ModLoader;

using VoidPort.Common;
using VoidPort.Content.Tiles.Void;
using VoidPort.Content.Generation.Helpers;

namespace VoidPort.Content.Generation
{
	public class VoidGen
	{
		//Generation stuff
		readonly static bool IsSmallWorld = Main.maxTilesX <= 4200;

		private static int PlaceBiomeX;
		private static int PlaceBiomeY;
		readonly static int IslandOffsetX = 70;
		readonly static int IslandFarOffsetX = 170;
		readonly static int IslandOffsetY = IsSmallWorld ? 25 : 40;
		
		readonly static float MagicFloat = 0.93335f;

		private static List<Point> IslandPositions; //The X and Y position of the islands
		private static List<Point> IslandSize; //The width and height of the island as Point
		private static List<Point> ValidTiles; //The valid tiles for the tile runner

		private static WorldGen.GrowTreeSettings TreeSettings;

		//Main method: Biome making
		public static void VoidIslands(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Obstructing the skies";

			//Initialize lists
			IslandPositions = [];
			IslandSize = [];
			ValidTiles = [];
			
			//Biome position
			PlaceBiomeX = (int)(Main.maxTilesX * MagicFloat);
			PlaceBiomeY = IsSmallWorld ? 75 : 120;
			
			Point VoidOrigin = new(PlaceBiomeX, PlaceBiomeY);

			//Island points
			if (!IsSmallWorld)
			{
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X + IslandOffsetX, VoidOrigin.Y + IslandOffsetY));
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X + IslandOffsetX, VoidOrigin.Y - IslandOffsetY));
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X + IslandFarOffsetX, VoidOrigin.Y));
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X - IslandOffsetX, VoidOrigin.Y + IslandOffsetY));
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X - IslandOffsetX, VoidOrigin.Y - IslandOffsetY));
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X - IslandFarOffsetX, VoidOrigin.Y));
			}
			else
			{
				IslandPositions.Add(GetIslandPoint(VoidOrigin.X, VoidOrigin.Y + IslandOffsetY, randY: false));
			}

			//Get sizes for the islands
			for (int i = 0; i < IslandPositions.Count; i++)
			{
				Point size = IsSmallWorld ? GetIslandSize(85, 95, 0.4f, true) : GetIslandSize(50, 60, 0.55f, true);
				IslandSize.Add(size);
			}

			//Base island generation
			foreach(Point Pos in IslandPositions)
			{
				//Get the index for the progress and the size
				int posIndex = IslandPositions.IndexOf(Pos);
				Point size = IslandSize[posIndex];

				//Island width and height values
				int width = size.X;
				int height = size.Y;

				//Values for tile runners
				int oreAmount = IsSmallWorld ? WorldGen.genRand.Next(7, 10) : WorldGen.genRand.Next(3, 6);
				int cracksAmount = IsSmallWorld ? WorldGen.genRand.Next(5, 9) : WorldGen.genRand.Next(1, 4);

				//Progress
				progress.Set((float)posIndex / IslandPositions.Count);

				//Task: Generate the island base
				GenerateIsland(Pos.X, Pos.Y, width, height);

				//Task: Place the soft surface of the island
				SoftSurface(Pos.X, Pos.Y, width + 2, height + 2);

				//Task: Generate valid points for the tile runners
				GenerateRunnerPoints(Pos.X, Pos.Y, width, height);

				//Task: Tile runner to place apocalyptite
				Runner(oreAmount, 5.25f, 15, 25, ModContent.TileType<Apocalyptite>());

				//Task: Tile runner to generate cracks
				Runner(cracksAmount, 3.75f, 75, 91, -1);

				//Task: Clean up odd tiles
				CleanUp(Pos.X, Pos.Y, width, height);
			}

			//Place the Strange Machine aka Zero deactivated
			Flags.whereIsZero = new Vector2(VoidOrigin.X * 16,(VoidOrigin.Y + 3) * 16);
		}

		//Main method: Biome ambience
		public static void VoidAmbience(GenerationProgress progress, GameConfiguration configuration)
		{
			int biomeWidth;
			int biomeHeight;

			if (Main.maxTilesX > 4200)
			{
				biomeWidth = 240;
				biomeHeight = 85;
			}
			else
			{
				biomeWidth = 105;
				biomeHeight = 70;
			}

			for (int x = PlaceBiomeX - biomeWidth; x <= PlaceBiomeX + biomeWidth; x++)
			{
				for (int y = PlaceBiomeY - biomeHeight; y <= PlaceBiomeY + biomeHeight; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);

					if (tile.TileType == (ushort)ModContent.TileType<SoftDoomstone>())
                    {
                        //grow trees
						if (WorldGen.genRand.NextBool(6))
						{
							TreeSettings = new WorldGen.GrowTreeSettings
							{
								GroundTest = (_) => true,
								WallTest = (_) => true,
								TreeHeightMax = 15,
								TreeHeightMin = 10,
								TreeTileType = TileID.Trees,
								TreeTopPaddingNeeded = 4,
							};

							WorldGen.GrowTreeWithSettings(x, y - 1, TreeSettings);
						}
					}
				}
			}
		}

		//Helper method: Get points for the islands
		private static Point GetIslandPoint(int i, int j, bool randX = true, bool randY = true)
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

		//Helper method: Get the sizes for the islands
		private static Point GetIslandSize(int minWidth, int maxWidth, float heightMult, bool constSize = false)
		{
			int width = constSize ? WorldGenTools.Average(minWidth, maxWidth) : WorldGen.genRand.Next(minWidth, maxWidth + 1);
			int height = (int)(width * heightMult);

			return new Point(width, height);
		}

		//Helper method: Generate islands
		private static void GenerateIsland(int cx, int cy, int width, int height)
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
				float topNoise = WorldGenTools.Perlin(x * 0.04f, seed, 3, 0.4f);
				int topOffset = (int)(topMask * (height * 0.3f) * topNoise);

				//Bottom
				float bottomMask = MathF.Pow(1f - normalized, sharpness); //Triangular
				float bottomNoise = WorldGenTools.Perlin(x * bottomFreq, unchecked(seed - 1), 5, 0.6f);
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
				for (int y = cy - (height / 3); y <= cy + height + 2; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);

					if (tile.WallType > WallID.None && WorldGenTools.IsTouchingAir(x, y)) 
					{
						WorldGen.KillWall(x, y);
					}
				}
			}
		}

		//Helper method: Soft surface for the islands
		public static void SoftSurface(int cx, int cy, int width, int height)
		{
			for (int x = cx - width; x <= cx + width; x++)
			{
				int softCount = 0;

				for (int y = cy + 2 - (height / 3); y < cy + height; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);

					if (tile.HasTile && WorldGen.SolidTile(x, y) && softCount++ < 5)
					{
						tile.TileType = (ushort)ModContent.TileType<SoftDoomstone>();

						if (tile.WallType > WallID.None)
						{
							tile.WallType = (ushort)ModContent.WallType<SoftDoomstoneWall>();
						}
					}
				}
			}
		}

		//Helper method: Generate valid points for the tile runner
		public static void GenerateRunnerPoints(int x, int y, int width, int height)
		{
			ValidTiles.Clear();

            for (int i = x - width; i <= x + width; i += 2)
			{
				for (int j = y - (height / 3); j <= y + height; j += 2)
				{
					if (Framing.GetTileSafely(i, j).HasTile)
					{
						int tileAmount = WorldGenTools.MooreTiles(i, j, 3);

						if (tileAmount >= 48)
						{
							ValidTiles.Add(new Point(i, j));
						}
					}
				}
			}
		}

		//Helper method: Tile runner (scary)
		public static void Runner(int amount, float strength, int rangeMin, int rangeMax, int type)
        {
			int numOres = amount;
			
			if (ValidTiles.Count > 0)
			{
				while(numOres > 0)
				{
					//Get the point
					int listIndex = WorldGen.genRand.Next(ValidTiles.Count);
					Point runnerPoint = ValidTiles[listIndex];
					ValidTiles.Remove(runnerPoint);
					
					//Runner time
					numOres--;
					WorldGen.TileRunner(runnerPoint.X, runnerPoint.Y, strength, WorldGen.genRand.Next(rangeMin, rangeMax), type);
				}
			}
		}

		//Helper method: Clean up
		public static void CleanUp(int cx, int cy, int width, int height)
		{
			for(int x = cx - width; x <= cx + width; x++)
			{
				for(int y = cy - height; y <= cy + height; y++)
				{
					if (Framing.GetTileSafely(x, y).HasTile)
					{
						//Kill tiles that are only connected to one tile
						bool tilesLeft = Framing.GetTileSafely(x - 1, y).HasTile && Framing.GetTileSafely(x - 2, y).HasTile;
						bool tilesRight = Framing.GetTileSafely(x + 1, y).HasTile && Framing.GetTileSafely(x + 2, y).HasTile;
						bool tilesUp = Framing.GetTileSafely(x, y - 1).HasTile && Framing.GetTileSafely(x, y - 2).HasTile;
						bool tilesBelow = Framing.GetTileSafely(x, y + 1).HasTile && Framing.GetTileSafely(x, y + 2).HasTile;

						if (!tilesLeft && !tilesRight && !tilesUp && !tilesBelow)
						{
							WorldGen.KillTile(x, y);
						}
					}
				}
			}
		}
	}
}