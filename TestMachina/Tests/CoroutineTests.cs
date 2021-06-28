using FluentAssertions;
using Machina.Data;
using Machina.Engine;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestMachina.Tests
{
    public class CoroutineTests
    {
        private class IsDoneFlag
        {
            public bool isDone = false;
        }

        /// <summary>
        /// One day we might want to change this behavior, but this is how Machina works today:
        /// If you yield return new WaitSeconds(0) it will always wait one frame, it will not instantly slide to the next one
        /// </summary>
        [Fact]
        public void one_yield_per_frame()
        {
            var scene = new Scene(null, null);
            IEnumerator<ICoroutineAction> InstantCoroutine(IsDoneFlag flag)
            {
                yield return new WaitSeconds(0.1f);
                yield return new WaitSeconds(0.1f);
                yield return new WaitSeconds(0.1f);
                flag.isDone = true;
                yield return null;
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
        public void nested_coroutines_work()
        {
            var scene = new Scene(null, null);
            IEnumerator<ICoroutineAction> OuterCoroutine(Scene scene, IsDoneFlag inner, IsDoneFlag outer)
            {
                yield return new WaitSeconds(1);
                yield return scene.StartCoroutine(InnerCoroutine(inner));
                yield return new WaitSeconds(1);
                outer.isDone = true;
                yield return null;
            }

            IEnumerator<ICoroutineAction> InnerCoroutine(IsDoneFlag inner)
            {
                yield return new WaitSeconds(1);
                yield return new WaitSeconds(1);
                yield return new WaitSeconds(1);
                inner.isDone = true;
                yield return null;
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
        }
    }
}
