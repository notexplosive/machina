namespace Machina.Engine.Input
{
    public class HitTester
    {
        public HitTestResult Candidate { get; private set; }

        public void AddCandidate(HitTestResult target)
        {
            if (target.depth.AsInt < Candidate.depth.AsInt)
            {
                Candidate = target;
            }
            else if (target.depth == Candidate.depth && target.actor != Candidate.actor)
            {
                // MachinaClient.Print("Z-fighting on hover at depth=", target.depth, target.actor, Candidate.actor);
            }
        }

        public void Clear()
        {
            Candidate = HitTestResult.Empty;
        }
    }
}