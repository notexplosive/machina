namespace Machina.Data.Layout
{
    public class OverflowRule
    {
        private OverflowRule(bool haltOnFailure, bool deletesWholeRow, bool doNotAddMoreRowsAfterFailure, bool loseFailingItem)
        {
            HaltImmediatelyUponFailure = haltOnFailure;
            DeletesWholeRowUponFailure = deletesWholeRow;
            DoNotAddMoreRowsAfterFailure = doNotAddMoreRowsAfterFailure;
            LoseFailingItem = loseFailingItem;
        }

        public static readonly OverflowRule Free = new OverflowRule(false, false, false, false);
        public static readonly OverflowRule HaltOnIllegal = new OverflowRule(true, false, true, true);
        public static readonly OverflowRule HaltOnIllegalButKeepLastOne = new OverflowRule(true, false, true, false);
        public static readonly OverflowRule CancelRowOnIllegal = new OverflowRule(true, true, true, true);
        public static readonly OverflowRule FinishRowOnIllegal = new OverflowRule(false, false, true, false);

        public bool HaltImmediatelyUponFailure { get; }
        public bool DeletesWholeRowUponFailure { get; }
        public bool DoNotAddMoreRowsAfterFailure { get; }
        public bool LoseFailingItem { get; }
    }
}
