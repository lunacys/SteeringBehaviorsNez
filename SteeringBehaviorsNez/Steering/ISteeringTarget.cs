using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez.Steering
{
    public interface ISteeringTarget
    {
        Vector2 Position { get; set; }
        bool IsActual { get; }
    }
}