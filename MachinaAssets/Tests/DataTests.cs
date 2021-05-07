using Machina.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Tests
{
    class DataTests : TestGroup
    {
        public DataTests() : base("Data Tests")
        {
            AddTest("UnorderedPair Tests", test =>
            {
                var pair1 = new UnorderedPair<Point>(new Point(0, 1), new Point(5, 5));
                var sameAsPair1 = new UnorderedPair<Point>(new Point(0, 1), new Point(5, 5));
                var p1 = new Point(5, 5);
                var p2 = new Point(0, 1);
                var pair1Flipped = new UnorderedPair<Point>(p1, p2);
                var pair2 = new UnorderedPair<Point>(new Point(0, -5), new Point(5, 5));

                test.ExpectTrue(pair1.Equals(pair1), "UnorderedPair.Equals works on self");
                test.ExpectTrue(pair1.Equals(sameAsPair1), "UnorderedPair.Equals works on identical set");
                test.ExpectTrue(pair1.Equals(pair1Flipped), "UnorderedPair.Equals works on flipped set");
                test.ExpectFalse(pair1.Equals(pair2), "UnorderedPair.Equals should be false for 2 different pairs");

                test.ExpectTrue(pair1 == sameAsPair1, "UnorderedPair == works on identical set");
                test.ExpectTrue(pair1 == pair1Flipped, "UnorderedPair == works on flipped set");
                test.ExpectFalse(pair1 == pair2, "UnorderedPair == should be false for 2 different pairs");
            });
        }
    }
}
