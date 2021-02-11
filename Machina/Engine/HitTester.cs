﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Machina.Engine
{
    public struct HitTestResult : IComparable<HitTestResult>, IEquatable<HitTestResult>
    {
        private static int IdPool;
        private readonly int id;
        public readonly Actor actor;
        public readonly Action<bool> approvalCallback;
        public readonly float depth;
        // Has an impossibly high depth of 2.0f
        public readonly static HitTestResult Empty = new HitTestResult(2.0f, null);

        public HitTestResult(Actor actor, Action<bool> callback) : this(actor.depth, callback)
        {
            this.actor = actor;
        }

        public HitTestResult(float depth, Action<bool> callback)
        {
            this.id = IdPool++;
            this.depth = depth;
            this.approvalCallback = callback;
            this.actor = null;
        }

        public int CompareTo(HitTestResult other)
        {
            if (other.depth == this.depth)
            {
                return 0;
            }

            return other.depth > this.depth ? -1 : 1;
        }

        public bool Equals(HitTestResult other)
        {
            return this.id == other.id;
        }


        public bool IsEmpty()
        {
            return Equals(Empty);
        }


    }

    public class HitTester
    {
        public HitTestResult Candidate
        {
            get; private set;
        }

        public void AddCandidate(HitTestResult target)
        {
            if (target.depth < Candidate.depth)
            {
                Candidate = target;
            }
            else if (target.depth == Candidate.depth && target.actor != Candidate.actor)
            {
                MachinaGame.Print("Z-fighting on hover at depth=", target.depth, target.actor, Candidate.actor);
            }
        }

        public void Clear()
        {
            Candidate = HitTestResult.Empty;
        }
    }
}