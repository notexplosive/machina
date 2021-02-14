using Machina.Components;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    internal class ActorTests
    {
        private class FakeComponent : BaseComponent
        {
            private Action onDeleteLambda;

            public FakeComponent(Actor actor, Action onDeleteLambda) : base(actor)
            {
                this.onDeleteLambda = onDeleteLambda;
            }

            public override void OnDelete()
            {
                this.onDeleteLambda();
            }
        }

        private readonly static List<TestResult> results = new List<TestResult>();

        public static void Run()
        {
            var scene = new Scene();
            var actor = scene.AddActor("Terry Triangle");
            var deleteCount = 0;
            new FakeComponent(actor, () => { deleteCount++; });
            actor.Destroy();

            // Scene needs to update to flush the destroyed actors
            scene.Update(0f);

            Expect(1, deleteCount, "Component is deleted once, when Actor is deleted.");

            Finish();
        }

        private static void Finish()
        {
            foreach (var result in results)
            {
                if (!result.IsPassing())
                {
                    MachinaGame.Print("TEST FAILURE: " + result.GetMessage());
                }
            }
        }

        public static void Expect<T>(T expected, T actual, string message) where T : struct
        {
            if (!actual.Equals(expected))
            {
                AddResult(new Failure<T>(expected, actual, message));
            }
            else
            {
                AddResult(new Pass());
            }
        }

        private static void AddResult(TestResult result)
        {
            results.Add(result);
        }

        public interface TestResult
        {
            bool IsPassing();
            string GetMessage();
        }

        public class Failure<T> : TestResult where T : struct
        {
            private readonly string message;

            public Failure(T expected, T actual, string preamble)
            {
                this.message = preamble + "\nExpected `" + expected.ToString() + "`, got " + actual.ToString();
            }

            public string GetMessage()
            {
                return this.message;
            }

            public bool IsPassing()
            {
                return false;
            }
        }

        public class Pass : TestResult
        {
            public string GetMessage()
            {
                return "Passed!";
            }

            public bool IsPassing()
            {
                return true;
            }
        }
    }
}
