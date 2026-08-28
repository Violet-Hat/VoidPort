using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace VoidPort.Content.Biomes.Void
{
    public class VoidBG : ModSurfaceBackgroundStyle
    {
        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
				}
				else
				{
					fades[i] -= transitionSpeed;
				}

                fades[i] = MathHelper.Clamp(fades[i], 0f, 1f);
            }
        }

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => -1;

        public override int ChooseMiddleTexture() => -1;

        public override int ChooseFarTexture() => -1;
    }
}