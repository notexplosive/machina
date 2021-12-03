using System.Collections.Generic;
using FluentAssertions;
using Machina.Data;
using Machina.Engine;
using Xunit;

namespace TestMachina.Tests
{
    public class CoroutineTests
    {
        /// <summary>
        ///     One day we might want to change this behavior, but this is how Machina works today:
        ///     If you yield return new WaitSeconds(0) it will always wait one frame, it will not instantly slide to the next one
        /// </summary>
        [Fact]
        public void one_yield_per_frame()
        {
            var scene = new Scene(null);

            IEnumerator<ICoroutineAction> InstantCoroutine(IsDoneFlag flag)
            {
                yield return new WaitSeconds(0.1f);
                yield return new WaitSeconds(0.1f);
                yield return new WaitSeconds(0.1f);
                flag.isDone = true;
            }

            var flag = new IsDoneFlag();
            scene.StartCoroutine(InstantCoroutine(flag));
            scene.Update(1f);

            flag.isDone.Should().BeFalse();

            scene.Update(1f);
            scene.Update(1f);
            flag.isDone.Should().BeTrue();
        }

        [Fact]
        public void ask_a_coroutine_if_done()
        {
            IEnumerator<ICoroutineAction> Subject(IsDoneFlag flag)
            {
                yield return new WaitSeconds(1);
                yield return new WaitSeconds(1);
                flag.isDone = true;
            }

            var flag = new IsDoneFlag();
            var scene = new Scene(null);
            var coroutine = scene.StartCoroutine(Subject(flag));

            var isDoneBeforeStart = coroutine.IsComplete(0);
            scene.Update(1);
            var isDoneInTheMiddle = coroutine.IsComplete(0);
            scene.Update(2);
            var isDoneAtEnd = coroutine.IsComplete(0);

            isDoneBeforeStart.Should().BeFalse();
            isDoneInTheMiddle.Should().BeFalse();
            isDoneAtEnd.Should().BeTrue();

        }

        [Fact]
        public void nested_coroutines_work()
        {
            var scene = new Scene(null);

            IEnumerator<ICoroutineAction> OuterCoroutine(Scene scene, IsDoneFlag inner, IsDoneFlag outer)
            {
                yield return new WaitSeconds(1);
                yield return scene.StartCoroutine(InnerCoroutine(inner));
                yield return new WaitSeconds(1);
                outer.isDone = true;
            }

            IEnumerator<ICoroutineAction> InnerCoroutine(IsDoneFlag inner)
            {
                yield return new WaitSeconds(1);
                yield return new WaitSeconds(1);
                yield return new WaitSeconds(1);
                inner.isDone = true;
            }

            var inner = new IsDoneFlag();
            var outer = new IsDoneFlag();

            scene.StartCoroutine(OuterCoroutine(scene, inner, outer));
            scene.Update(1f);
            scene.Update(1f);
            scene.Update(1f);
            scene.Update(1f);

            outer.isDone.Should().BeFalse();
            inner.isDone.Should().BeTrue();

            scene.Update(1f);
            scene.Update(1f);

            outer.isDone.Should().BeTrue();
        }

        private class IsDoneFlag
        {
            public bool isDone;
        }
    }
}