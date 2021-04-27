using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering.Behaviors
{
    public class Flee : SteeringComponentBase
    {
        public override Vector2 Steer(ISteeringTarget target)
        {
            var dv = (target.Position - SteeringEntity.Position);
            dv.Normalize();
            dv *= SteeringEntity.MaxVelocity;
            
            SteeringEntity.DesiredVelocity = -dv;

            return SteeringEntity.DesiredVelocity - SteeringEntity.Velocity;
        }
    }
}