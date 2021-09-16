using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.Engine.Debugging.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine.Debugging.Components
{
    public class DebugDock : BaseComponent
    {
        public readonly WindowManager windowManager;
        private readonly LayoutGroup layoutGroup;
        private readonly int rowHeight = 90;
        private readonly int iconWidth = 90;
        private readonly int titleHeight = 32;
        private readonly int dockMargin = 5;
        private readonly int iconPadding = 5;

        private LayoutGroup currentRow;
        private readonly InvokableDebugTool invokable;

        public DebugDock(Actor actor) : base(actor)
        {
            this.invokable = RequireComponent<InvokableDebugTool>();
            var boundingRect = RequireComponent<BoundingRect>();
            boundingRect.SetSize(new Point(iconWidth * 3 + iconPadding * 2 + dockMargin * 2, rowHeight * 2 + titleHeight + dockMargin * 2));

            this.layoutGroup = RequireComponent<LayoutGroup>();
            this.layoutGroup.AddHorizontallyStretchedElement("Title", titleHeight, titleActor =>
            {
                new BoundedTextRenderer(titleActor, "Machina Debug Dock", MachinaGame.defaultStyle.uiElementFont, Color.White, HorizontalAlignment.Center, VerticalAlignment.Center);
            })
            .SetMarginSize(new Point(dockMargin, dockMargin));

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
            this.layoutGroup.AddHorizontallyStretchedElement("Row", rowHeight, rowActor =>
            {
                row = new LayoutGroup(rowActor, Orientation.Horizontal)
                    .SetPaddingBetweenElements(iconPadding);
            });
            return row;
        }

        public void AddApp(App app)
        {
            if (this.currentRow.transform.ChildCount >= 3)
            {
                this.currentRow = AddRow();
            }

            LayoutGroup row = this.currentRow;
            row.AddVerticallyStretchedElement("IconRootActor", iconWidth, iconActor =>
            {
                new Hoverable(iconActor);
                new Clickable(iconActor);
                var doubleClickable = new DoubleClickable(iconActor);
                doubleClickable.DoubleClick += (button) =>
                {
                    if (button == MouseButton.Left)
                    {
                        app.Open(this.actor.scene.sceneLayers.debugScene, windowManager);
                    }
                };

                new DebugIconHoverRenderer(iconActor);
                new LayoutGroup(iconActor, Orientation.Vertical)
                   .SetMarginSize(new Point(5, 5))
                   .AddHorizontallyStretchedElement("IconImage", 50, iconImageActor =>
                   {
                       new BoundingRectFill(iconImageActor, Color.Orange);
                   })
                   .AddBothStretchedElement("IconText", iconTextActor =>
                   {
                       new BoundedTextRenderer(iconTextActor, app.appName, MachinaGame.Assets.GetSpriteFont("TinyFont"), Color.White, HorizontalAlignment.Center, VerticalAlignment.Top);
                   });
            });
        }
    }
}
