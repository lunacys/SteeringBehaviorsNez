using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering
{
    public abstract class SteeringBehaviorBase : ISteeringBehavior
    {
        public ISteeringEntity SteeringEntity { get; set; }

        public abstract Vector2 Steer(ISteeringTarget target);
    }
}