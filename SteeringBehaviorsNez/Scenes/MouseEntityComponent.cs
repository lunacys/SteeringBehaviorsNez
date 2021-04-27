using Microsoft.Xna.Framework;
using Nez;
using SteeringBehaviorsNez.Steering;

namespace SteeringBehaviorsNez.Scenes
{
    public class MouseEntityComponent : Component, IUpdatable, ISteeringTarget
    {
        Vector2 ISteeringTarget.Position
        {
            get => Entity.Position;
            set => Entity.Position = value;
        }

        public bool IsActual => true;

        public void Update()
        {
            Entity.Position = Input.MousePosition;
        }
    }
}