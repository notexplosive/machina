using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;

namespace Machina.Engine.Debugging.Components
{
    public class DebugDock : BaseComponent
    {
        private readonly int dockMargin = 5;
        private readonly int iconPadding = 5;
        private readonly int iconWidth = 90;
        private readonly InvokableDebugTool invokable;
        private readonly LayoutGroup layoutGroup;
        private readonly int rowHeight = 90;
        private readonly int titleHeight = 32;
        public readonly WindowManager windowManager;

        private LayoutGroup currentRow;

        public DebugDock(Actor actor) : base(actor)
        {
            this.invokable = RequireComponent<InvokableDebugTool>();
            var boundingRect = RequireComponent<BoundingRect>();
            boundingRect.SetSize(new Point(this.iconWidth * 3 + this.iconPadding * 2 + this.dockMargin * 2,
                this.rowHeight * 2 + this.titleHeight + this.dockMargin * 2));

            this.layoutGroup = RequireComponent<LayoutGroup>();
            this.layoutGroup.AddHorizontallyStretchedElement("Title", this.titleHeight,
                    titleActor =>
                    {
                        new BoundedTextRenderer(titleActor, "Machina Debug Dock",
                            MachinaGame.defaultStyle.uiElementFont, Color.White, HorizontalAlignment.Center,
                            VerticalAlignment.Center);
                    })
                .SetMarginSize(new Point(this.dockMargin, this.dockMargin));

            this.windowManager = new WindowManager(MachinaGame.defaultStyle, this.actor.transform.Depth - 100);

            this.currentRow = AddRow();
        }

        public void Close()
        {
            this.invokable.Close();
        }

        private LayoutGroup AddRow()
        {
            LayoutGroup row = null;
            this.layoutGroup.AddHorizontallyStretchedElement("Row", this.rowHeight, rowActor =>
            {
                row = new LayoutGroup(rowActor, Orientation.Horizontal)
                    .SetPaddingBetweenElements(this.iconPadding);
            });
            return row;
        }

        public void AddApp(App app)
        {
            if (this.currentRow.transform.ChildCount >= 3)
            {
                this.currentRow = AddRow();
            }

            var row = this.currentRow;
            row.AddVerticallyStretchedElement("IconRootActor", this.iconWidth, iconActor =>
            {
                new Hoverable(iconActor);
                new Clickable(iconActor);
                var doubleClickable = new DoubleClickable(iconActor);
                doubleClickable.DoubleClick += button =>
                {
                    if (button == MouseButton.Left)
                    {
                        app.Open(this.actor.scene.sceneLayers.debugScene, this.windowManager);
                    }
                };

                new DebugIconHoverRenderer(iconActor);
                new LayoutGroup(iconActor, Orientation.Vertical)
                    .SetMarginSize(new Point(5, 5))
                    .AddHorizontallyStretchedElement("IconImage", 50,
                        iconImageActor => { new BoundingRectFill(iconImageActor, Color.Orange); })
                    .AddBothStretchedElement("IconText",
                        iconTextActor =>
                        {
                            new BoundedTextRenderer(iconTextActor, app.appName,
                                MachinaGame.Assets.GetSpriteFont("TinyFont"), Color.White, HorizontalAlignment.Center);
                        });
            });
        }
    }
}
