using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    abstract class TestGroup
    {
        private static List<TestGroup> registry = new List<TestGroup>();
        public static void Register(TestGroup group)
        {
            if (!registry.Contains(group))
            {
                registry.Add(group);
            }
        }

        public int totalPassed { private set; get; } = 0;
        public string groupName;
        public List<Test> tests = new List<Test>();
        public readonly List<Test> failingTests = new List<Test>();

        public TestGroup(string name)
        {
            this.groupName = name;
            registry.Add(this);
        }

        public void AddTest(Test test)
        {
            tests.Add(test);
        }

        public void AddTest(string name, Action<Test> lambda)
        {
            tests.Add(new Test(name, lambda));
        }

        public TestGroup RunAll()
        {
            // Run all the tests
            foreach (var test in tests)
            {
                test.Run();
            }

            // Analyze results
            foreach (var test in tests)
            {
                bool wasFailed = false;
                foreach (var result in test.results)
                {
                    if (!result.IsPassing())
                    {
                        wasFailed = true;
                    }
                }

                if (wasFailed)
                {
                    this.failingTests.Add(test);
                }

                if (!wasFailed)
                {
                    this.totalPassed++;
                }
            }

            // Display failing tests:
            if (failingTests.Count > 0)
            {
                MachinaGame.Print(this.groupName, "had", failingTests.Count, "failures");
            }

            foreach (var test in failingTests)
            {
                MachinaGame.Print("", test.name);
                foreach (var result in test.results)
                {
                    if (!result.IsPassing())
                    {
                        MachinaGame.Print("  ", result.GetMessage());
                    }
                }
            }

            return this;
        }

        public static int RunAllRegisteredTests()
        {
            int totalPassed = 0;
            foreach (var group in registry)
            {
                group.RunAll();
                totalPassed += group.totalPassed;
            }
            return totalPassed;
        }
    }

    struct Test
    {
        public readonly List<ITestResult> results;
        public readonly string name;
        private readonly Action<Test> lambda;

        public Test(string name, Action<Test> lambda)
        {
            this.results = new List<ITestResult>();
            this.name = name;
            this.lambda = lambda;
        }

        public void Run()
        {
            this.lambda(this);
        }

        public bool CheckEqual<T>(T expected, T actual)
        {
            if (actual == null)
            {
                if (expected == null)
                {
                    return true;
                }
            }

            if (expected == null)
            {
                return actual.Equals(expected);
            }

            return expected.Equals(actual);
        }

        public void Expect<T>(T expected, T actual, string message = "")
        {
            if (!CheckEqual(expected, actual))
            {
                AddResult(new Failure<T>(expected, actual, message));
            }
            else
            {
                AddResult(new Pass());
            }
        }

        public void Expect(float expected, float actual, string message)
        {
            bool isEqual = Math.Abs(expected - actual) < 0.0001f;
            if (!isEqual)
            {
                AddResult(new Failure<float>(expected, actual, message));
            }
            else
            {
                AddResult(new Pass());
            }
        }

        public void Expect(Vector2 expected, Vector2 actual, string message)
        {
            bool isEqual = Math.Abs(expected.X - actual.X) < 0.0001f && Math.Abs(expected.Y - actual.Y) < 0.0001f;
            if (!isEqual)
            {
                AddResult(new Failure<Vector2>(expected, actual, message));
            }
            else
            {
                AddResult(new Pass());
            }
        }


        public void ExpectNot<T>(T expected, T actual, string message = "")
        {
            if (CheckEqual(expected, actual))
            {
                AddResult(new Failure<T>(expected, actual, message));
            }
            else
            {
                AddResult(new Pass());
            }
        }

        private void AddResult(ITestResult result)
        {
            results.Add(result);
        }

        internal void ExpectNotNull(object notNull, string message = "")
        {
            ExpectNot(null, notNull, message);
        }

        internal void ExpectNull(object probablyNull, string message = "")
        {
            Expect(null, probablyNull, message);
        }

        internal void ExpectFalse(bool condition, string message)
        {
            Expect(false, condition, message);
        }

        internal void ExpectTrue(bool condition, string message)
        {
            Expect(true, condition, message);
        }
    }

    public interface ITestResult
    {
        bool IsPassing();
        string GetMessage();
    }

    public class Failure<T> : ITestResult
    {
        private readonly string message;

        public Failure(T expected, T actual, string preamble)
        {
            var expectedString = expected != null ? expected.ToString() : "null";
            var actualString = actual != null ? actual.ToString() : "null";
            this.message = preamble + (preamble != "" ? ": " : "") + "Expected `" + expectedString + "`, got " + "`" + actualString + "`";
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

    public class Pass : ITestResult
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
