using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.GameContent;

namespace VoidPort.Content.Biomes.Void
{
    public class VoidSky : CustomSky
    {
        private static bool skyActive;
        private static float opacity;

        public override void Update(GameTime gameTime)
        {
            if (skyActive && opacity < 1f)
            {
                opacity += 0.01f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.02f;
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            skyActive = false;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D skyTexture = ModContent.Request<Texture2D>("VoidPort/Content/Biomes/Void/VoidSky").Value;

            if (maxDepth >= 3E+38f && minDepth < 3E+38f && !Main.gameMenu)
            {
                //Draw the sky box texture
                spriteBatch.Draw(
                    skyTexture,
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                    Color.White * opacity);
                
                //Star folly
                if (!Main.dedServ)
                {
                    float colorMult = 0.95f * opacity;
                    
                    //Most of these values are taken from Vanilla code
                    for (int i = 0; i < Main.star.Length; i++)
                    {
                        Star star = Main.star[i];
                        if (star == null) continue;

                        //Texture
                        Texture2D tex = TextureAssets.Star[star.type].Value;

                        //Position
                        Vector2 starVect = new(star.position.X / 1920f, star.position.Y / 1200f);
                        Vector2 sceneArea = new(Main.screenWidth, Main.screenHeight);
                        Vector2 position = starVect * sceneArea;

                        //Origin
                        Vector2 origin = tex.Size() * 0.5f;

                        //Draw
                        spriteBatch.Draw(
                            tex,
                            position,
                            new Rectangle(0, 0, tex.Width, tex.Height),
                            Color.Red * star.twinkle * colorMult,
                            star.rotation,
                            origin,
                            star.scale * star.twinkle,
                            SpriteEffects.None,
                            0f);
                    }
                }
            }

            //Deactivate sky on menu or if the player doesn't exist inside the world
            if (Main.gameMenu || !Main.LocalPlayer.active)
            {
                skyActive = false;
            }
        }

        public override float GetCloudAlpha()
		{
			return 1f - opacity;
		}
    }
}