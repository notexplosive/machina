using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Assets;
using Machina.Engine.Cartridges;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Examples
{
    public class ExampleGame : GameCartridge
    {
        public ExampleGame() : base(new Point(640, 480), ResizeBehavior.KeepAspectRatio)
        {
        }

        public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
        {
            var gameScene = SceneLayers.AddNewScene();

            var platform = gameScene.AddActor("Platform", new Vector2(200, 300));
            new BoundingRect(platform, new Point(300, 32))
                .SetOffsetToCenter();
            new BoundingRectFill(platform, Color.White);

            var playerActor = gameScene.AddActor("Player", new Vector2(200, 200));
            var playerSprites = MachinaClient.Assets.GetMachinaAsset<GridBasedSpriteSheet>("player");
            new BoundingRect(playerActor, new Point(64, 32));
            new SpriteRenderer(playerActor, playerSprites)
                .SetupBoundingRect();
            // new PlayerController(playerActor);


            var uiScene = SceneLayers.AddNewScene();
            var healthActor = uiScene.AddActor("Healthbar", new Vector2(10, 10));
            new BoundingRect(healthActor, new Point(200, 20));
            // new HealthbarRenderer(healthActor);
        }

        public override void PrepareDynamicAssets(AssetLoader loader, GraphicsDevice graphicsDevice, MachinaRuntime runtime)
        {
            // Assumes Content has a SpriteFont called "my-font"
            // Assumes Content has a Texture called "player-sprite"
            loader.AddMachinaAssetCallback("player", () => new GridBasedSpriteSheet("player-sprite", new Point(64, 64)));
        }
    }
}
