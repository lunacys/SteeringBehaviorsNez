using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering.Behaviors
{
    public class Pursuit : SteeringComponentBase
    {
        private Seek _seek;

        public override void Initialize()
        {
            base.Initialize();

            _seek = new Seek();
            _seek.SteeringEntity = SteeringEntity;
        }

        public override Vector2 Steer(ISteeringTarget target)
        {
            if (target == null)
                return Vector2.Zero;

            var distance = (target.Position - SteeringEntity.Position).Length();
            var updatesAhead = distance / SteeringEntity.MaxVelocity;
            Vector2 futurePos;
            if (target is ISteeringEntity steeringTarget)
                futurePos = target.Position + steeringTarget.Velocity * updatesAhead;
            else 
                futurePos = target.Position;

            return _seek.Steer((Vector2SteeringTarget) futurePos);
        }
    }
}