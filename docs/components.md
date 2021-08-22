# Components Overview

## What are they?

If you're coming from Unity, Components should feel very familiar. A component is a chunk of behavior attached to an Actor (Actor == GameObject). Each component has several overridable entry-point methods such as `Update`, `Draw`, `OnMousePress`

## How do I make one?

At minimum a component must look like this:

```cs
using Machina.Components;
using Machina.Engine;

class MyComponent : BaseComponent
{
    public MyComponent(Actor actor) : base(actor)
    {
        // Your initialization code here.
    }
}
```

Every component **must** use the `base(actor)` constructor. You _may_ add additional args your component's constructor, as long as `base(actor)` is called, you're good!

## How do I use it?

Assuming you're in a fresh project and haven't done anything in Machina before, let's go into your game's `OnGameLoad` method.

```cs
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace SOMENAMESPACE
{
    public class Game1 : MachinaGame
    {
        public Game1(string[] args) : base("My Cool Game", args, new Point(1920, 1080), new Point(1600, 900), ResizeBehavior.MaintainDesiredResolution)
        {
        }

        protected override void OnGameLoad()
        {
            // 1
            var gameScene = SceneLayers.AddNewScene();

            // 2
            var myActor = gameScene.AddActor("My Cool Actor");

            // 3
            myActor.transform.Position = new Vector2(300, 300);

            // 4
            new BoundingRect(myActor, new Point(50, 50))
                // 5
                .SetOffsetToCenter();

            // 6
            new BoundingRectFill(myActor, Color.White);
        }
    }
}
```

Let's take a closer look at this:

1. We use the `SceneLayers` property of MachinaGame to add a new `Scene`. By default we have zero scenes.
2. We add an `Actor` to the scene, their name is `"My Cool Actor"`, we could have named them anything.
3. We set the `Actor`'s position via it's `Transform`. `Actor.transform` is a Property of Actor, but it's _also_ a component that is added by default.
4. `BoundingRect` is a built-in component, generally used to describe an actor's bounding box. Calling the constructor of a component with an `Actor` passed into the first param is how we attach components to actors. It is the _only_ way. There is a method called `Actor.AddComponent`, **DO NOT USE IT**.
5. `BoundingRect` has a method on it called `SetOffsetToCenter` which makes the rect centered around the actor's position. By default the actor's position is the top left corner of the rect.
6. Now we create a second component, a `BoundingRectFill`. This is a simple component that just draws a filled rectangle wherever the BoundingRect is. If we did this before we attached a BoundingRect, it would have failed an assert and crashed.

## How do they talk to each other?

Let's take a closer look at `BoundingRectFill`.

```cs
// Slightly abridged version of Machina.Components.BoundingRectFill

public class BoundingRectFill : BaseComponent
{
    private readonly BoundingRect boundingRect;
    private readonly Color color;

    public BoundingRectFill(Actor actor, /*1*/ Color color) : base(actor)
    {
        /*2*/
        this.boundingRect = RequireComponent<BoundingRect>();
        this.color = color;
    }

    /*3*/
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.FillRectangle(this.boundingRect.Rect, this.color, /*4*/transform.Depth);
    }
}
```

1. Note how `BoundingRectFill` has a 2 parameter constructor, but still uses `base(actor)`. So long as `base(actor)` is called you're free to do whatever you want with the constructor.

2. To obtain the BoundingRect, we call `RequireComponent<BoundingRect>()`, this does two things. It returns a copy of the current actor's BoundingRect _and_ it throws a DebugAssert if it doesn't find one. This means that if it fails at runtime, you'll get an easy-to-follow callstack. If it fails in release mode though, all bets are off.

   - As an aside, if you want to get a component _without_ the assert (meaning a null value is acceptable), use `actor.GetComponent<T>`

3. Here we override the `Draw` method which takes a SpriteBatch. You might need to read about how SpriteBatch works from the MonoGame docs. In this case I'm using the `MonoGame.Extended` plugin to draw a rectangle.

4. Normally we'd supply a `depth` value here, which is a float from `0.0` to `1.0` that describes who renders on top of who. This is a massive pain in the ass. So instead I invented a new type `Depth` that works like an integer that maps to `0.0` to `1.0`. Transform has a `Depth` property which is the current depth of the actor. Most draw calls will use this depth value unless it makes sense to use something different.
