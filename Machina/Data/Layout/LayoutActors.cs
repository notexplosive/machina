using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.Layout
{
    public class LayoutActors
    {
        private readonly Dictionary<string, Actor> actorTable = new Dictionary<string, Actor>();
        private readonly Actor rootActor;

        public LayoutActors(Scene scene, LayoutNode rootNode, Point position = default)
        {
            var layout = new LayoutBaker(rootNode).Bake();

            var actorName = rootNode.Name.Text;
            this.rootActor = scene.AddActor(actorName);
            new BoundingRect(this.rootActor, layout.GetNode(actorName).Size);

            AddActorToTable(actorName, this.rootActor);
            SetupRootActor(this.rootActor, actorName, layout, position.ToVector2());
            CreateActorsForChildren(rootNode, layout);
        }

        private void CreateActorsForChildren(LayoutNode parent, BakedLayout layout)
        {
            if (!parent.HasChildren)
            {
                return;
            }

            var parentActor = GetActor(parent.Name.Text);

            foreach (var childNode in parent.Children)
            {
                if (childNode.Name.Exists)
                {
                    var actorName = childNode.Name.Text;
                    var actor = parentActor.transform.AddActorAsChild(actorName);

                    new LayoutSiblingWithCachedOrientation(actor, parent.Orientation);
                    new BoundingRect(actor, layout.GetNode(actorName).Size);

                    AddActorToTable(actorName, actor);
                    SetupChildActor(actor, actorName, layout);
                    CreateActorsForChildren(childNode, layout);
                }
            }
        }

        private void SetupChildActor(Actor actor, string actorName, BakedLayout layout)
        {
            var bakedLayoutNode = layout.GetNode(actorName);
            var size = bakedLayoutNode.Size;
            actor.GetComponent<BoundingRect>().SetSize(size); // we might want to cache the Component lookup for perf
            actor.transform.Position = this.rootActor.transform.Position + bakedLayoutNode.PositionRelativeToRoot.ToVector2();
            actor.transform.LocalDepth = -bakedLayoutNode.NestingLevel;

        }

        private void SetupRootActor(Actor actor, string actorName, BakedLayout layout, Vector2 absolutePosition)
        {
            actor.GetComponent<BoundingRect>().SetSize(layout.GetNode(actorName).Size); // we might want to cache the Component lookup for perf
            actor.transform.Position = absolutePosition;
        }

        /// <summary>
        /// Using the existing pool of actors, but a new layout, change the hierarchy
        /// </summary>
        /// <param name="resizedRootNode"></param>
        public void ReapplyLayout(LayoutNode resizedRootNode)
        {
            var newLayout = new LayoutBaker(resizedRootNode).Bake();
            foreach (var actorName in newLayout.AllResultNodeNames())
            {
                var actor = this.actorTable[actorName];
                if (actor == this.rootActor)
                {
                    SetupRootActor(actor, actorName, newLayout, this.rootActor.transform.Position);
                }
                else
                {
                    SetupChildActor(actor, actorName, newLayout);
                }
            }
        }

        private void AddActorToTable(string text, Actor actor)
        {
            actorTable[text] = actor;
        }

        public Actor GetActor(string actorName)
        {
            return this.actorTable[actorName];
        }
    }
}
