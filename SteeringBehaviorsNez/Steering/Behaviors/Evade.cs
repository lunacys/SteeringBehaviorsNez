using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering.Behaviors
{
    public class Evade : SteeringComponentBase
    {
        public override Vector2 Steer(ISteeringTarget target)
        {
            var distance = (target.Position - SteeringEntity.Position).Length();
            var updatesAhead = distance / SteeringEntity.MaxVelocity;
            Vector2 futurePos;
            if (target is ISteeringEntity steeringTarget)
                futurePos = target.Position + steeringTarget.Velocity * updatesAhead;
            else 
                futurePos = target.Position;

            return BehaviorMath.Flee((Vector2SteeringTarget) futurePos, SteeringEntity);
        }
    }
}