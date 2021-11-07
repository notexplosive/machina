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
        private readonly Action onFinish;
        private float delayTime;
        private bool readyToFinish;
        private bool isTotallyDone;

        public LoadingScreen(AssetLoadTree tree, Action onFinish)
        {
            delayTime = 0.25f;
            this.tree = tree;
            this.onFinish = onFinish;
        }

        public void Increment(AssetLibrary assetLibrary, float dt)
        {
            if (isTotallyDone)
            {
                return;
            }

            if (delayTime > 0)
            {
                delayTime -= dt;
                return;
            }

            if (readyToFinish)
            {
                onFinish();
                isTotallyDone = true;
                return;
            }

            if (tree.IsDoneLoading() && !readyToFinish)
            {
                readyToFinish = true;
                delayTime = 0.3f;
                return;
            }

            tree.LoadNextThing(assetLibrary);
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
            var progress = tree.Progress();

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
    }
}