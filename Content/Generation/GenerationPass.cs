using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using System.Collections.Generic;

namespace VoidPort.Content.Generation
{
    public class GenerationPass : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            //Add the Void biome in the worldgen task
            int VoidIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if(VoidIndex != -1)
            {
                tasks.Insert(VoidIndex + 1, new PassLegacy("Void Islands", VoidGen.VoidIslands));
                tasks.Insert(VoidIndex + 2, new PassLegacy("Void Ambience", VoidGen.VoidAmbience));
            }
        }
    }
}