using System.Runtime.InteropServices;
using System.Text;

namespace EngineWrapper
{
    public class Pinvoke
    {
        [DllImport("AppEngine")]
        public static extern int GetBidFromRule(Phase phase, string hand, int lastBidId, int position, int[] minSuitsPartner, int[] minSuitsOpener, 
            string previousBidding, string previousSlemBidding, bool isCompetitive, int minHcpPartner, bool allControlsPresent, out Phase newPhase, StringBuilder description);
        [DllImport("AppEngine", CharSet = CharSet.Ansi)]
        public static extern void GetRulesByBid(Phase phase, int bidId, int position, string previousBidding, bool isCompetitive, StringBuilder information);
        [DllImport("AppEngine", CharSet = CharSet.Ansi)]
        public static extern void GetRelativeRulesByBid(int bidId, string previousBidding, StringBuilder information);
        [DllImport("AppEngine")]
        public static extern void SetModules(int modules);
    }
}
