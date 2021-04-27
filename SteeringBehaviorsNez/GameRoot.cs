using Nez;
using SteeringBehaviorsNez.Scenes;

namespace SteeringBehaviorsNez
{
    public class GameRoot : Core
    {
        protected override void Initialize()
        {
            base.Initialize();

            Scene = new SteeringBehaviorsScene();
            DebugRenderEnabled = true;
        }
    }
}