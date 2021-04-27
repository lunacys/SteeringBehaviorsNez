using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;
using SteeringBehaviorsNez.Steering;

namespace SteeringBehaviorsNez.Scenes
{
    public class SteeringBehaviorsScene : Scene
    {
        public const int ScreenSpaceRenderLayer = 999;

        private ImGuiManager _imGuiManager;

        public SteeringBehaviorsScene()
        {
            AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));
            AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer));
        }

        public override void Initialize()
        {
            base.Initialize();

            _imGuiManager = new ImGuiManager();
            Core.RegisterGlobalManager(_imGuiManager);

            ClearColor = Color.CornflowerBlue;

            SetDesignResolution(1360, 768, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(1920, 1080);

            CreateEntity("mouse-entity").AddComponent(new MouseEntityComponent());
            var path = CreateEntity("path-builder").AddComponent(new PathComponent(new Path()));
            CreateEntity("sb-base").AddComponent(new SteeringBehaviors(path));
        }
    }
}