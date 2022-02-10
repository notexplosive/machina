namespace Machina.Data.TextRendering
{
    public readonly struct TextOutputFragment
    {
        public TextOutputFragment(string token, IFontMetrics fontMetrics)
        {
            Text = token;
            FontMetrics = fontMetrics;
        }

        public string Text { get; }
        public IFontMetrics FontMetrics { get; }
    }
}
