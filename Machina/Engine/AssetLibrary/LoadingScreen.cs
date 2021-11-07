namespace Machina.Engine.AssetLibrary
{
    using System;

    public class LoadingScreen
    {
        private readonly AssetLoadTree tree;
        private readonly Action onFinish;
        private float delayTime;

        public LoadingScreen(AssetLoadTree tree, Action onFinish)
        {
            this.delayTime = 1f;
            this.tree = tree;
            this.onFinish = onFinish;
        }

        public bool IsDone()
        {
            return this.tree.IsDoneLoading();
        }

        public void Increment(AssetLibrary assetLibrary, float dt)
        {
            this.delayTime -= dt;

            if (delayTime > 0)
            {
                return;
            }
            
            this.tree.LoadNextThing(assetLibrary);
            Console.WriteLine(MathF.Floor(this.tree.Progress() * 100) + "%");

            if (this.tree.IsDoneLoading())
            {
                this.onFinish();
            }
        }
    }
}