namespace Machina.Data.Layout
{
    public class OverflowRule
    {
        private OverflowRule(bool hasInfiniteRows, bool allowExtraRoomOnLastRow)
        {
            HasInfiniteRows = hasInfiniteRows;
            AllowExtraRoomOnLastRow = allowExtraRoomOnLastRow;
        }

        public static readonly OverflowRule Free = new OverflowRule(true, false);
        public static readonly OverflowRule LastRowKeepsGoing = new OverflowRule(false, true);
        public static readonly OverflowRule EverythingMustBeInside = new OverflowRule(false, false);

        public bool HasInfiniteRows { get; }
        public bool AllowExtraRoomOnLastRow { get; }
    }
}
