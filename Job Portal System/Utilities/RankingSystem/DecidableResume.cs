using Job_Portal_System.Utilities.RankingSystem.Abstracts;

namespace Job_Portal_System.Utilities.RankingSystem
{
    internal class DecidableResume : Decidable
    {
        private readonly double[] _row;
        private readonly bool _accepted;

        public DecidableResume(double[] row, bool accepted = false)
        {
            _row = row;
            _accepted = accepted;
        }

        public override object[] GetRow()
        {
            var res = new object[_row.Length + 1];
            _row.CopyTo(res, 0);
            res[_row.Length] = _accepted ? "YES" : "NO";
            return res;
        }
    }
}
