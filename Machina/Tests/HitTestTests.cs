using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class KeySimulator : KeyTracker
    {
        private Keys[] pressed;
        private Keys[] released;
        private ModifierKeys modifiers;

        public void Set(Keys[] pressed, Keys[] released, ModifierKeys modifiers)
        {
            this.pressed = pressed;
            this.released = released;
            this.modifiers = modifiers;
        }

        public override void Calculate()
        {
            this.Modifiers = modifiers;
            this.Pressed = pressed;
            this.Released = released;
        }
    }

    class MouseSimulator : MouseTracker
    {
        public override void Calculate()
        {
            this.ButtonsPressedThisFrame = pressed;
            this.ButtonsReleasedThisFrame = ButtonsReleasedThisFrame;
        }
    }

    class ScrollSimulator : ScrollTracker
    {
        public override void Calculate()
        {
        }
    }

    class HitTestTests : TestGroup
    {
        public HitTestTests() : base("HitTestTests")
        {
            AddTest(new Test("HitTesting", test =>
            {
                var sceneLayers = new SceneLayers(new KeySimulator(), new MouseSimulator(), new ScrollSimulator());
                var scene = new Scene();
                sceneLayers.Add(scene);

                sceneLayers.Update(0, Matrix.Identity);

                var depthAt1Hit = false;
                scene.hitTester.AddCandidate(new HitTestResult(1.0f, b => { depthAt1Hit = true; }));
                scene.OnMouseUpdate(new Point(200, 200), new Vector2(10, 10), new Vector2(10, 10));
                test.Expect(HitTestResult.Empty, scene.hitTester.Candidate, "HitTestResult is cleared by OnMouseUpdate");
            }));
        }
    }
}

