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

        public static OverflowRule PermitExtraRows = new OverflowRule(false, false, false, false);
        public static OverflowRule HaltOnIllegal = new OverflowRule(true, false, true, false);
        public static OverflowRule CancelRowOnIllegal = new OverflowRule(true, true, true, true);
        public static OverflowRule FinishRowOnIllegal = new OverflowRule(false, false, true, false);

        public bool HaltImmediatelyUponFailure { get; }
        public bool DeletesWholeRowUponFailure { get; }
        public bool DoNotAddMoreRowsAfterFailure { get; }
        public bool LoseFailingItem { get; }
    }
}
