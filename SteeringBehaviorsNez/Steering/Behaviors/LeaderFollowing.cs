using Microsoft.Xna.Framework;
using Nez;

namespace SteeringBehaviorsNez.Steering.Behaviors
{
    public class LeaderFollowing : SteeringComponentBase
    {
        public ISteeringEntity Leader { get; set; }
        [Inspectable]
        public float LeaderBehindDist { get; set; }

        public float LeaderSightRadius { get; set; }

        private float _slowingRadius;

        private Arrival _arrival;

        private Vector2 _behind, _ahead;

        public LeaderFollowing(ISteeringEntity leader, float leaderBehindDist, float slowingRadius, float leaderSightRadius)
        {
            Leader = leader;
            LeaderBehindDist = leaderBehindDist;
            _slowingRadius = slowingRadius;
            LeaderSightRadius = leaderSightRadius;
        }

        public override void Initialize()
        {
            base.Initialize();

            _arrival = new Arrival(_slowingRadius); // TODO: Add as Nested Behavior
            _arrival.SteeringEntity = SteeringEntity;
        }

        public override Vector2 Steer(ISteeringTarget target)
        {
            var dv = Leader.Velocity;
            var force = Vector2.Zero;

            dv.Normalize();
            dv *= LeaderBehindDist;
            _ahead = Leader.Position + dv;

            dv *= -1;
            _behind = Leader.Position + dv;

            //if (IsOnLeaderSight(_ahead))
            //    force += _evade.Steer(Leader as ISteeringTarget);

           return force + _arrival.Steer((Vector2SteeringTarget) _behind);
        }

        private bool IsOnLeaderSight(Vector2 leaderAhead)
        {
            return Vector2.Distance(leaderAhead, SteeringEntity.Position) <= LeaderSightRadius ||
                   Vector2.Distance(Leader.Position, SteeringEntity.Position) <= LeaderSightRadius;
        }

        public override void DebugRender(Batcher batcher)
        {
            batcher.DrawCircle(_behind, 8f, Color.Green);
            batcher.DrawCircle(_behind, _slowingRadius, Color.GreenYellow);
        }
    }
}