using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering.Behaviors
{
    public class Seek : SteeringComponentBase
    {
        public override Vector2 Steer(ISteeringTarget target)
        {
            var dv = target.Position - SteeringEntity.Position;
            dv.Normalize();

            SteeringEntity.DesiredVelocity = dv;

            return (SteeringEntity.DesiredVelocity * SteeringEntity.MaxVelocity) - SteeringEntity.Velocity;
        }
    }
}