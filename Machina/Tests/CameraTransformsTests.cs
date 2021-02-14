using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class CameraTransformsTests : TestGroup
    {
        public CameraTransformsTests() : base("Camera Transforms")
        {
            AddTest("", test =>
            {
                var sceneLayers = new SceneLayers(null);
                var scene = new Scene();
                sceneLayers.Add(scene);
                var mouseState = new MouseState(200, 200, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
                sceneLayers.Update(0, Matrix.Identity, new InputState(mouseState, new KeyboardState()));
            });
        }
    }
}
