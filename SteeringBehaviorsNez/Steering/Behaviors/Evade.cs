using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering.Behaviors
{
    public class Evade : SteeringComponentBase
    {
        private Flee _flee;

        public override void Initialize()
        {
            base.Initialize();

            _flee = new Flee();
            _flee.SteeringEntity = SteeringEntity;
        }

        public override Vector2 Steer(ISteeringTarget target)
        {
            var distance = (target.Position - SteeringEntity.Position).Length();
            var updatesAhead = distance / SteeringEntity.MaxVelocity;
            Vector2 futurePos;
            if (target is ISteeringEntity steeringTarget)
                futurePos = target.Position + steeringTarget.Velocity * updatesAhead;
            else 
                futurePos = target.Position;

            return _flee.Steer((Vector2SteeringTarget) futurePos);
        }
    }
}