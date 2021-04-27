using System;
using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering
{
    public interface ISteeringBehavior
    {
        ISteeringEntity SteeringEntity { get; set; } 
        Vector2 Steer(ISteeringTarget target);
        Func<ISteeringEntity, ISteeringTarget, bool> Condition { get; set; }
    }
}