using Microsoft.Xna.Framework;
using Nez;
using Nez.ImGuiTools;

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

            
        }
    }
}