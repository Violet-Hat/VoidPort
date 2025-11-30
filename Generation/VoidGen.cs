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
		public static ushort Doomstone = (ushort)ModContent.TileType<Doomstone>(),
		SoftDoomstone = (ushort)ModContent.TileType<SoftDoomstone>(),
		Apocalyptite = (ushort)ModContent.TileType<Apocalyptite>(),
		DoomstoneWall = (ushort)ModContent.WallType<DoomstoneWall>();
		
		//Biome generation
		private void VoidIslands(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Obstructing the skies";
			
			List<Point> IslandPositions = new List<Point>();
			
			int PlaceBiomeX = (int)(Main.maxTilesX - (Main.maxTilesX / 15));
			int PlaceBiomeY = 120;
			
			Point VoidOrigin = new Point(PlaceBiomeX, PlaceBiomeY);
			
			IslandPositions.Add(getIslandPoint(VoidOrigin.X + 52, VoidOrigin.Y + 42));
			IslandPositions.Add(getIslandPoint(VoidOrigin.X + 52, VoidOrigin.Y - 42));
			IslandPositions.Add(getIslandPoint(VoidOrigin.X + 135, VoidOrigin.Y));
			IslandPositions.Add(getIslandPoint(VoidOrigin.X - 52, VoidOrigin.Y + 42));
			IslandPositions.Add(getIslandPoint(VoidOrigin.X - 52, VoidOrigin.Y - 42));
			IslandPositions.Add(getIslandPoint(VoidOrigin.X - 135, VoidOrigin.Y));
			
			foreach(var Position in IslandPositions)
			{
				int width = WorldGen.genRand.Next(70, 80);
				int height = WorldGen.genRand.Next(20, 30);
			
				GenerateIsland(Position, width, height);
			}
			
			//Spawn Zero aka Strange Machine
			Flags.whereIsZero = new Vector2(VoidOrigin.X * 16, VoidOrigin.Y * 16);
			int strangeMachine = NPC.NewNPC(null, (int)Flags.whereIsZero.X, (int)Flags.whereIsZero.Y, ModContent.NPCType<ZeroDeactivated>());
			Main.npc[strangeMachine].position.X -= 7;
			Main.npc[strangeMachine].position.Y -= 4;
		}
		
		private Point getIslandPoint(int i, int j)
		{
			int offsetX = WorldGen.genRand.Next(-5, 5);
			int offsetY = WorldGen.genRand.Next(-5, 5);
			
			int x = i + offsetX;
			int y = j + offsetY;
			
			return new Point(x, y);
		}
		
		private void GenerateIsland(Point position, int width, int height)
        {	
			//Size numbers
			int Num1 = (int)(width / 2);
			int Num2 = (int)(Num1 / 3);
			int Num3 = Num2 * 2;
			int Num4 = (int)(width / 10);
			int Num5 = (int)(height / 3);
			int Num6 = Num5 * 2;
			int Num7 = (int)(height / 10);
			
			//Control points
			Vector2 c1 = new Vector2(position.X - Num1 - 2, position.Y);
			Vector2 c2 = new Vector2(position.X, position.Y + height);
			Vector2 c3 = new Vector2(position.X + Num1 + 2, position.Y);
			
			//In between control Points, b1 -> b5 are for the bottom part while b6 & b7 are for the top part
			Vector2 b1 = new Vector2(position.X - Num3, position.Y + Num7);
			Vector2 b2 = new Vector2(position.X - Num4, position.Y + Num6);
			Vector2 b4 = new Vector2(position.X + Num4, position.Y + Num6);
			Vector2 b5 = new Vector2(position.X + Num3, position.Y + Num7);
			Vector2 b6 = new Vector2(position.X - Num2, position.Y - Num5);
			Vector2 b7 = new Vector2(position.X + Num2, position.Y - Num5);
			
			//Left to right
			GenerateCurves(position, 10000, c1, b1, b2, c2, false);
			
			//Right to left
			GenerateCurves(position, 10000, c2, b4, b5, c3, false);
			
			//Top
			c1 = new Vector2(position.X - Num1, position.Y);
			c3 = new Vector2(position.X + Num1, position.Y);
			
			GenerateCurves(position, 10000, c1, b6, b7, c3, true);
			
			//Generate spikes
			Point spikesOrigin = new Point((int)c1.X, (int)c1.Y);
			GenSpikes(spikesOrigin, width, height);
			
			//Generate apocalyptite
			Ores(position, Num3, Num6, WorldGen.genRand.Next(3, 5), 4f, 30, 40, true);
			
			//Place softStone an walls
			int nX = position.X - Num1 - 4;
			int nY = position.Y - Num5;
			int sizeX = width + 6;
			int sizeY = Num5 + 1;
			
			Point newOrigin = new Point(nX, nY);
			
			WorldUtils.Gen(newOrigin, new Shapes.Rectangle(sizeX , sizeY), Actions.Chain(new GenAction[]
			{
				new Modifiers.Blotches(2, 0.5),
				new Modifiers.IsSolid(),
				new Actions.SetTile(SoftDoomstone, true),
			}));
			
			sizeY = height + 6;
			
			WorldUtils.Gen(newOrigin, new Shapes.Rectangle(sizeX , sizeY), Actions.Chain(new GenAction[]
			{
				new Modifiers.IsSolid(),
				new Actions.PlaceWall(DoomstoneWall),
				new Modifiers.IsTouchingAir(true),
				new Actions.ClearWall(),
			}));
			
			//Generate caves
			Ores(position, Num2, Num5, WorldGen.genRand.Next(5, 8), 2.75f, 60, 90, false);
        }
		
		private void GenerateCurves(Point position, int segments, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, bool check)
		{
			for(int i = 0; i < segments; i++)
			{
                float t = i / (float)segments;
				Vector2 placing = WorldgenTools.cubicBezier(t, p0, p1, p2, p3);
				
				int X = (int)placing.X;
				int Y = (int)placing.Y;
				
				if(check)
				{
					for(int j = Y; j < position.Y; j++)
					{
						WorldGen.PlaceTile(X, j, Doomstone);
					}
				}
				else
				{
					for(int j = position.Y; j <= Y; j++)
					{
						WorldGen.PlaceTile(X, j, Doomstone);
					}
				}
            }
		}
		
		private void GenSpikes(Point origin, int width, int height)
		{
			int Num8 = (int)(width / 6);
			int Num9 = Num8 * 2;
			int Num10 = Num8 * 4;
			int Num11 = Num8 * 5;
			
			int Num12 = (int)(height / 4);
			int Num13 = (int)(Num12 * 2.5f);
			int Num14 = (int)(Num12 * 3.5f);
			
			int spikeSize1 = (int)(width * 0.1f);
			int spikeSize2 = (int)(width * 0.15f);
			
			int spikeRand = (int)(width * 0.09f);
			
			Spikes(new Point(origin.X + Num8, origin.Y), spikeSize1, Num13 + WorldGen.genRand.Next(0, spikeRand));
			Spikes(new Point(origin.X + Num9, origin.Y), spikeSize2, Num14 + WorldGen.genRand.Next(0, spikeRand));
			Spikes(new Point(origin.X + Num10, origin.Y), spikeSize2, Num14 + WorldGen.genRand.Next(0, spikeRand));
			Spikes(new Point(origin.X + Num11, origin.Y), spikeSize1, Num13 + WorldGen.genRand.Next(0, spikeRand));
		}
		
		private void Spikes(Point position, int width, int size)
		{
			Vector2D end = new Vector2D(0, size);
			
			WorldUtils.Gen(position, new Shapes.Tail(width, end), Actions.Chain(new GenAction[]
			{
				new Actions.PlaceTile(Doomstone),
			}));
		}
		
		private void Ores(Point position, int width, int height, int amount, float strength, int rangeMin, int rangeMax, bool placeOre)
        {
			int numOres = amount;
			int i = 0;
			
			int x;
			int y;
			
			while(numOres > 0 && i++ < 10000)
			{
				x = WorldGen.genRand.Next(position.X - width, position.X + width);
				y = WorldGen.genRand.Next(position.Y, position.Y + height);
				
				if(Main.tile[x, y].HasTile)
				{
					numOres--;
					if(placeOre)
					{
						WorldGen.TileRunner(x, y, strength, WorldGen.genRand.Next(rangeMin, rangeMax), Apocalyptite);
					}
					else
					{
						WorldGen.TileRunner(x, y, strength, WorldGen.genRand.Next(rangeMin, rangeMax), -1);
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