using System;
using System.Collections.Generic;
using Machina.Engine;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Machina.Components
{
    public class SimplePhysicsBody : BaseComponent
    {
        public enum BodyType
        {
            Static,
            Dynamic
        }

        public readonly BodyType bodyType;
        private readonly BoundingRect boundingRect;
        private readonly List<CollideMoment> collisionsThisFrame;

        public SimplePhysicsBody(Actor actor, BodyType bodyType) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.bodyType = bodyType;
            this.collisionsThisFrame = new List<CollideMoment>();
        }

        public Vector2 Velocity { get; set; }

        public RectangleF ColliderRect => this.boundingRect.RectF;

        /// <summary>
        ///     Assuming this is a dynamic body, resolve a collision against this other body
        /// </summary>
        /// <param name="other"></param>
        public void CollideWith(SimplePhysicsBody other, float dt)
        {
            var otherCollider = other.ColliderRect;
            if (DynamicRectVsRect(this, otherCollider, out var contactPoint, out var contactNormal,
                out var contactTime, dt))
            {
                this.collisionsThisFrame.Add(new CollideMoment(contactTime, otherCollider, contactNormal));
            }
        }

        public override void Update(float dt)
        {
            transform.Position += Velocity * dt;
        }

        public override void OnPostUpdate()
        {
            if (this.bodyType == BodyType.Dynamic)
            {
                ResolveCollisions();
            }
        }

        private void ResolveCollisions()
        {
            this.collisionsThisFrame.Sort((a, b) => { return (int) ((a.contactTime - b.contactTime) * 1000); });

            foreach (var collision in this.collisionsThisFrame)
            {
                Velocity += collision.contactNormal * new Vector2(MathF.Abs(Velocity.X), MathF.Abs(Velocity.Y)) *
                            (1f - collision.contactTime);
            }

            this.collisionsThisFrame.Clear();
        }

        /// <summary>
        ///     Adapted from OLC Tutorial
        /// </summary>
        /// <param name="body"></param>
        /// <param name="target"></param>
        /// <param name="contactPoint"></param>
        /// <param name="contactNormal"></param>
        /// <param name="contactTime"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool DynamicRectVsRect(SimplePhysicsBody body, RectangleF target, out Vector2 contactPoint,
            out Vector2 contactNormal, out float contactTime, float dt)
        {
            contactPoint = Vector2.Zero;
            contactNormal = Vector2.Zero;
            contactTime = 0f;

            if (body.Velocity == Vector2.Zero)
            {
                return false;
            }

            var bodyRect = body.ColliderRect;
            var expandedTarget = new RectangleF(target.Position - bodyRect.Size / 2, target.Size + bodyRect.Size);

            if (RayVsRect(bodyRect.Center, body.Velocity * dt, expandedTarget, out contactPoint,
                out contactNormal, out contactTime))
            {
                if (contactTime < 1f && contactTime >= 0f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Adapted from OLC tutorial
        /// </summary>
        /// <param name="rayOrigin">Origin of the ray</param>
        /// <param name="rayDirection">Direction and magnitude of the Ray</param>
        /// <param name="rect">Rectangle of interest</param>
        /// <param name="contactPoint">Point in space where ray hits rect</param>
        /// <param name="contactNormal">Normal of contact site</param>
        /// <param name="hitNear">Magnitude along ray in which intersects with Rect</param>
        /// <returns></returns>
        public static bool RayVsRect(Vector2 rayOrigin, Vector2 rayDirection, RectangleF rect, out Vector2 contactPoint,
            out Vector2 contactNormal, out float hitNear)
        {
            contactNormal = Vector2.Zero;
            contactPoint = Vector2.Zero;

            var near = (rect.TopLeft - rayOrigin) / rayDirection;
            var far = (rect.BottomRight - rayOrigin) / rayDirection;

            if (float.IsNaN(far.X) || float.IsNaN(far.Y) || float.IsNaN(near.X) || float.IsNaN(near.Y))
            {
                hitNear = 0f;
                return false;
            }

            if (near.X > far.X)
            {
                var t = near.X;
                near.X = far.X;
                far.X = t;
            }

            if (near.Y > far.Y)
            {
                var t = near.Y;
                near.Y = far.Y;
                far.Y = t;
            }

            hitNear = MathF.Max(near.X, near.Y);
            var hitFar = MathF.Min(far.X, far.Y);

            if (hitFar < 0)
            {
                return false;
            }

            contactPoint = rayOrigin + hitNear * rayDirection;

            if (near.X > near.Y)
            {
                if (rayDirection.X < 0)
                {
                    contactNormal = new Vector2(1, 0);
                }
                else
                {
                    contactNormal = new Vector2(-1, 0);
                }
            }
            else if (near.X < near.Y)
            {
                if (rayDirection.Y < 0)
                {
                    contactNormal = new Vector2(0, 1);
                }
                else
                {
                    contactNormal = new Vector2(0, -1);
                }
            }

            return !(near.X > far.Y || near.Y > far.X || hitFar < 0);
        }

        private struct CollideMoment
        {
            public readonly RectangleF colliderRect;
            public readonly Vector2 contactNormal;
            public readonly float contactTime;

            public CollideMoment(float contactTime, RectangleF collider, Vector2 contactNormal)
            {
                this.contactTime = contactTime;
                this.colliderRect = collider;
                this.contactNormal = contactNormal;
            }
        }
    }
}