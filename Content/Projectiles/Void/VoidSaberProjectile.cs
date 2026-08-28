using Microsoft.Xna.Framework;

namespace VoidPort.Content.Projectiles.Void
{
    public class VoidSaberProjectile : BaseSwingProjectile
    {
        public override string Texture => "VoidPort/Content/Projectiles/BaseSwingProjectile";

        public override float ScaleMulti => 0.35f;

        public override Color ParticleColor1 => Color.Red;
        public override Color ParticleColor2 => Color.DarkRed;

        public override Color DrawColorBack => new(180, 60, 81);
        public override Color DrawColorMiddle => new(255, 80, 80);
        public override Color DrawColorFront => new(255, 150, 150);
    }
}