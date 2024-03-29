﻿using System.Collections.Generic;
using FluentAssertions;
using Machina.Internals;
using Xunit;

namespace TestMachina.Tests
{
    public class ForEachTest
    {
        [Fact]
        public void generic_for_each()
        {
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};

            Functions.ResilientForEach(items, item => { item.Visit(); });

            items.Should().BeEquivalentTo(new List<Visitable>
                {new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1)});
        }

        [Fact]
        public void for_each_that_appends_during_loop()
        {
            var iterationCount = 0;
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};

            Functions.ResilientForEach(items, item =>
            {
                item.Visit();
                iterationCount++;
                if (iterationCount == 2)
                {
                    items.Add(new Visitable());
                }
            });

            items.Should().BeEquivalentTo(new List<Visitable>
                {new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1)});
        }

        [Fact]
        public void for_each_that_prepends_during_loop()
        {
            var iterationCount = 0;
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};

            Functions.ResilientForEach(items, item =>
            {
                item.Visit();
                iterationCount++;
                if (iterationCount == 2)
                {
                    items.Insert(0, new Visitable());
                }
            });

            items.Should().BeEquivalentTo(new List<Visitable>
                {new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1)});
        }

        [Fact]
        public void for_each_that_inserts_during_loop()
        {
            var iterationCount = 0;
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};

            Functions.ResilientForEach(items, item =>
            {
                item.Visit();
                iterationCount++;
                if (iterationCount == 2)
                {
                    items.Insert(2, new Visitable());
                }
            });

            items.Should().BeEquivalentTo(new List<Visitable>
                {new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1)});
        }

        [Fact]
        public void for_each_that_removes_during_loop()
        {
            var iterationCount = 0;
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};

            Functions.ResilientForEach(items, item =>
            {
                item.Visit();
                iterationCount++;
                if (iterationCount == 2)
                {
                    items.RemoveAt(2);
                }
            });

            items.Should().BeEquivalentTo(new List<Visitable> {new Visitable(1), new Visitable(1), new Visitable(1)});
        }

        [Fact]
        public void for_each_that_removes_earlier_item_during_loop()
        {
            var iterationCount = 0;
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};
            var copyOfOriginalItems = new List<Visitable>(items);

            Functions.ResilientForEach(items, item =>
            {
                item.Visit();
                iterationCount++;
                if (iterationCount == 2)
                {
                    items.RemoveAt(0);
                }
            });

            items.Should().BeEquivalentTo(new List<Visitable> {new Visitable(1), new Visitable(1), new Visitable(1)});

            // All of the items got visited, even the one that was removed
            copyOfOriginalItems.Should().BeEquivalentTo(new List<Visitable>
                {new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(1)});
        }

        [Fact]
        public void for_each_that_removes_later_item_during_loop()
        {
            var iterationCount = 0;
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};
            var copyOfOriginalItems = new List<Visitable>(items);

            Functions.ResilientForEach(items, item =>
            {
                item.Visit();
                iterationCount++;
                if (iterationCount == 2)
                {
                    items.RemoveAt(3);
                }
            });

            items.Should().BeEquivalentTo(new List<Visitable> {new Visitable(1), new Visitable(1), new Visitable(1)});

            // All of the items got visited, except the one that was removed
            copyOfOriginalItems.Should().BeEquivalentTo(new List<Visitable>
                {new Visitable(1), new Visitable(1), new Visitable(1), new Visitable(0)});
        }

        [Fact]
        public void for_each_that_inserts_every_iteration()
        {
            var items = new List<Visitable> {new Visitable(), new Visitable(), new Visitable(), new Visitable()};
            Functions.TooManyIterationsException capturedException = null;

            try
            {
                Functions.ResilientForEach(items, item =>
                {
                    item.Visit();
                    items.Insert(2, new Visitable());
                });
            }
            catch (Functions.TooManyIterationsException s)
            {
                capturedException = s;
            }

            capturedException.Should().NotBeNull();
        }

        public class Visitable
        {
            public Visitable()
            {
                VisitCount = 0;
            }

            public Visitable(int i)
            {
                VisitCount = i;
            }

            public int VisitCount { get; private set; }

            public void Visit()
            {
                VisitCount++;
            }
        }
    }
}