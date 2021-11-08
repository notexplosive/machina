namespace Machina.Engine.AssetLibrary
{
    using System;
    using Data;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using MonoGame.Extended;

    public class LoadingScreen
    {
        private readonly AssetLoadTree tree;
        private readonly Action onFinishUpdating;
        private float delayTime;
        private bool readyToFinish;
        private bool isDoneUpdating;

        public LoadingScreen(AssetLoadTree tree, Action onFinishUpdating)
        {
            this.delayTime = 0.25f;
            this.tree = tree;
            this.onFinishUpdating = onFinishUpdating;
        }

        public void Update(AssetLibrary assetLibrary, float dt)
        {
            if (this.isDoneUpdating)
            {
                return;
            }

            if (this.delayTime > 0)
            {
                this.delayTime -= dt;
                return;
            }

            if (this.readyToFinish)
            {
                this.onFinishUpdating();
                this.isDoneUpdating = true;
                return;
            }

            if (this.tree.IsDoneUpdateLoading() && !this.readyToFinish)
            {
                this.readyToFinish = true;
                this.delayTime = 0.3f;
                return;
            }

            this.tree.UpdateLoadNextThing(assetLibrary);
        }

        public void Draw(SpriteBatch spriteBatch, Point currentWindowSize, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Begin();

            graphicsDevice.DepthStencilState = new DepthStencilState {DepthBufferEnable = true};
            graphicsDevice.Clear(Color.Black);

            var windowSize = currentWindowSize;
            var center = currentWindowSize.ToVector2() / 2f;
            var middleDepth = Depth.Middle;
            var offset = new Vector2(windowSize.X / 4f, 0);
            var startPoint = center - offset;
            var barThickness = 10f;
            var barBackgroundColor = Color.DarkRed;
            var barForegroundColor = Color.Orange;
            var progress = this.tree.Progress();

            spriteBatch.DrawCircle(new CircleF(startPoint, barThickness / 2f), 10, barBackgroundColor,
                barThickness / 2f, middleDepth);

            spriteBatch.DrawCircle(new CircleF(startPoint + offset * 2, barThickness / 2f), 10, barBackgroundColor,
                barThickness / 2f, middleDepth);

            spriteBatch.DrawLine(
                startPoint,
                startPoint + offset * 2,
                barBackgroundColor, barThickness, middleDepth
            );

            spriteBatch.DrawLine(
                startPoint,
                startPoint + offset * 2 * progress,
                barForegroundColor, barThickness, middleDepth - 1
            );

            if (progress > 0)
            {
                spriteBatch.DrawCircle(new CircleF(startPoint, barThickness / 2f), 10, barForegroundColor,
                    barThickness / 2f, middleDepth - 1);

                spriteBatch.DrawCircle(new CircleF(startPoint + offset * 2 * progress, barThickness / 2f), 10,
                    barForegroundColor,
                    barThickness / 2f, middleDepth - 1);
            }

            spriteBatch.End();
        }

        public void IncrementDrawLoopLoad(AssetLibrary assetLibrary, SpriteBatch spriteBatch)
        {
            if (this.tree.IsDoneDrawLoading())
            {
                return;
            }

            this.tree.DrawLoadNextThing(assetLibrary, spriteBatch);
        }

        public bool IsDoneDrawLoading()
        {
            return this.tree.IsDoneDrawLoading();
        }
    }
}