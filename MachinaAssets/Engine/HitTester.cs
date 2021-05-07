using Machina.Data;
using System;
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
        public readonly Depth depth;
        public readonly static HitTestResult Empty = new HitTestResult(new Depth(Depth.MaxAsInt), null);

        public HitTestResult(Actor actor, Action<bool> callback) : this(actor.transform.Depth, callback)
        {
            this.actor = actor;
        }

        public HitTestResult(Depth depth, Action<bool> callback)
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

            return other.depth.AsInt > this.depth.AsInt ? -1 : 1;
        }

        public bool Equals(HitTestResult other)
        {
            return this.id == other.id;
        }


        public bool IsEmpty()
        {
            return Equals(Empty);
        }

        public override string ToString()
        {
            if (IsEmpty())
            {
                return "Empty";
            }

            if (this.actor != null)
            {
                return this.actor.ToString() + " depth=" + depth;
            }

            return id.ToString() + " depth=" + depth;
        }

        internal static void ApproveTopCandidate(Scene[] scenes)
        {
            var willApproveCandidate = true;
            // Traverse scenes in reverse draw order (top to bottom)
            for (int i = scenes.Length - 1; i >= 0; i--)
            {
                var scene = scenes[i];
                var candidate = scene.hitTester.Candidate;
                if (!candidate.IsEmpty())
                {
                    candidate.approvalCallback?.Invoke(willApproveCandidate);
                    willApproveCandidate = false;
                }
            }
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
            if (target.depth.AsInt < Candidate.depth.AsInt)
            {
                Candidate = target;
            }
            else if (target.depth == Candidate.depth && target.actor != Candidate.actor)
            {
                // MachinaGame.Print("Z-fighting on hover at depth=", target.depth, target.actor, Candidate.actor);
            }
        }

        public void Clear()
        {
            Candidate = HitTestResult.Empty;
        }
    }
}
