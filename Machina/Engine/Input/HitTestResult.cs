using System;
using Machina.Data;

namespace Machina.Engine.Input
{
    public struct HitTestResult : IComparable<HitTestResult>, IEquatable<HitTestResult>
    {
        private static int IdPool;
        private readonly int id;
        public readonly Actor actor;
        public readonly Action<bool> approvalCallback;
        public readonly Depth depth;
        public static readonly HitTestResult Empty = new HitTestResult(new Depth(Depth.MaxAsInt), null);

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
                return this.actor + " depth=" + this.depth;
            }

            return this.id + " depth=" + this.depth;
        }

        internal static void ApproveTopCandidate(Scene[] scenes)
        {
            var willApproveCandidate = true;
            // Traverse scenes in reverse draw order (top to bottom)
            for (var i = scenes.Length - 1; i >= 0; i--)
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
}