using System;

namespace Machina.Data.Layout
{
    public struct LayoutNodeName
    {
        private readonly string internalString;
        public bool IsNameless => this.internalString == null;

        public LayoutNodeName(string text)
        {
            this.internalString = text;
        }

        public static implicit operator LayoutNodeName(string text)
        {
            return new LayoutNodeName(text);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.internalString);
        }

        public string Text
        {
            get
            {
                if (Exists)
                {
                    return this.internalString;
                }

                throw new Exception("Node does not have a name (is spacer or null)");
            }
        }

        public bool Exists => this.internalString != null;

        public static LayoutNodeName Nameless => new LayoutNodeName(null);

        public override string ToString()
        {
            if (Exists)
            {
                return this.internalString;
            }
            else
            {
                return "(nameless)";
            }
        }
    }
}
