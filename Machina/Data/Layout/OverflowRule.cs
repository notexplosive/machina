namespace Machina.Data.Layout
{
    public class OverflowRule
    {
        private OverflowRule(bool haltOnFailure, bool deletesWholeRow, bool doNotAddMoreRowsAfterFailure)
        {
            HaltImmediatelyUponFailure = haltOnFailure;
            DeletesWholeRowUponFailure = deletesWholeRow;
            DoNotAddMoreRowsAfterFailure = doNotAddMoreRowsAfterFailure;
        }

        public static OverflowRule PermitExtraRows = new OverflowRule(false, false, false);
        public static OverflowRule HaltOnIllegal = new OverflowRule(true, false, true);
        public static OverflowRule CancelRowOnIllegal = new OverflowRule(true, true, true);
        public static OverflowRule FinishRowOnIllegal = new OverflowRule(false, false, true);

        public bool HaltImmediatelyUponFailure { get; }
        public bool DeletesWholeRowUponFailure { get; }
        public bool DoNotAddMoreRowsAfterFailure { get; }
    }
}
