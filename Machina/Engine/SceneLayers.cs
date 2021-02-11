using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class SceneLayers
    {
        public readonly Scene debugScene;
        private readonly List<Scene> sceneList = new List<Scene>();

        public SceneLayers(Scene debugScene = null)
        {
            this.debugScene = debugScene;
        }

        public void Add(Scene scene)
        {
            this.sceneList.Add(scene);
        }

        public Scene[] AllScenes()
        {
            Scene[] array = new Scene[sceneList.Count + (debugScene != null ? 1 : 0)];
            sceneList.CopyTo(array);
            array[sceneList.Count] = debugScene;
            return array;
        }
    }
}
