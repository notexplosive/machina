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
// MyCoolGame.cs - abridged just to OnGameLoad

protected override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
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
```

Let's take a closer look at this:

1. We use the `SceneLayers` property of `GameCartridge` to add a new `Scene`. A scene is basically a "world" where all of our actors live. You can add multiple scenes to `SceneLayers` and they'll all render on top of each other. For example, you might want multiple scenes for your gameplay and UI.
2. We add an `Actor` to the scene, their name is `"My Cool Actor"`, we could have named them anything. Actors are like unity `GameObject`s. They're a hub of Components.
3. We set the `Actor`'s position via it's `Transform`. `Actor.transform` is a Property of Actor, but it's _also_ a component that is added by default and put in an easy-to-access property.

   - If you're not familiar with Unity, `transform` is a component that holds the Position, Angle, Parent, and Children of the actor. In Unity it would also store a "Scale" but I don't do that because I've yet to find a compelling reason to.

4. `BoundingRect` is a built-in component (aka: it's a completely normal component that I supply for you), generally used to describe an actor's bounding box. Calling the constructor of a component with an `Actor` passed into the first param is how we attach components to actors. It is the _only_ way.
5. `BoundingRect` has a method on it called `SetOffsetToCenter` which makes the rect centered around the actor's position. By default the actor's position is the top left corner of the rect.
6. Now we create a second component, a `BoundingRectFill`. This is a simple component that just draws a filled rectangle wherever the BoundingRect is. If we did this before we attached a BoundingRect, it would have failed an assert and crashed.

## How do they talk to each other?

Let's take a closer look at `BoundingRectFill`.

```cs
// Slightly abridged version of Machina.Components.BoundingRectFill circa 08/2021

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

1. Note how `BoundingRectFill` has a 2 parameter constructor, but still uses `base(actor)`. So long as `base(actor)` is called you're free to do whatever you want with the constructor. In this case, we ask for a color. We initialize components in their constructor so we get the benefit of the `readonly` keyword.

2. To obtain the BoundingRect, we call `RequireComponent<BoundingRect>()`, this does two things. It returns a copy of the current actor's BoundingRect _and_ it throws a DebugAssert if it doesn't find one. This means that if it fails at runtime, you'll get an easy-to-follow callstack. If it fails in release mode though, all bets are off.

   - As an aside, if you want to get a component _without_ the assert (meaning a null value is acceptable), use `actor.GetComponent<T>`

3. Here we override the `Draw` method which takes a SpriteBatch. You might need to read about how SpriteBatch works from the MonoGame docs. In this case I'm using the `MonoGame.Extended` plugin to draw a rectangle.

4. Normally we'd supply a `depth` value here, which is a float from `0.0` to `1.0` that describes who renders on top of who. Depth as a "percentage" is a massive pain in the ass. So instead I created a type `Depth` that can be treated like an integer but maps to `0.0` to `1.0`. Transform has a `Depth` property which is the current depth of the actor. Most draw calls will use this depth value unless it makes sense to use something different.

## How do I use assets?

```cs
// Assumes Content has a Texture called "images/player-sprite.png" (it needs to be in `images/` to be loaded)
// Assumes you have created a Component called PlayerController
// Assumes you have created a Component called HealthbarRenderer

public override void PrepareDynamicAssets(AssetLoader loader, GraphicsDevice graphicsDevice)
{
    // 1
    loader.AddMachinaAssetCallback("player", () => new GridBasedSpriteSheet("player-sprite", new Point(64, 64)));
}

// MyCoolGame.cs, abridged to just OnGameLoad and PrepareDynamicAssets
public override void OnGameLoad(GameSpecification specification, MachinaRuntime runtime)
{
    var gameScene = SceneLayers.AddNewScene();

    var platform = gameScene.AddActor("Platform", new Vector2(200, 300));
    new BoundingRect(platform, new Point(300, 32))
        .SetOffsetToCenter();
    new BoundingRectFill(platform, Color.White);

    var playerActor = gameScene.AddActor("Player", new Vector2(200, 200));
    // 2
    var playerSheet = MachinaClient.Assets.GetMachinaAsset<GridBasedSpriteSheet>("player");
    new BoundingRect(playerActor, new Point(64, 32));
    new SpriteRenderer(playerActor, playerSheet)
    new PlayerController(playerActor);

    // 3
    var uiScene = SceneLayers.AddNewScene();

    var healthActor = uiScene.AddActor("Healthbar", new Vector2(10, 10));
    new BoundingRect(healthActor, new Point(200, 20));
    new HealthbarRenderer(healthActor);
}
```

1. This adds a callback function to the loading screen that creates a GridBasedSpriteSheet from the `images/player-sprite.png` texture. We could technically do this in `OnGameLoad`, but some assets can be quite expensive to build so it's best to put all asset loading in PrepareDynamicAssets so they happen during the loading screen. We assign the asset to the string `"player"` so we can fetch it later.
2. We use the string `"player"` to fetch the GridBasedSpriteSheet. By this point it's totally ready to use.
3. Just as an example, we add a whole second scene, this will be rendered on top of the `gameScene` and is completely disjoint from it. The only way that uiScene and gameScene will interact is that HitTests from `uiScene` will block hitTests from `gameScene`
