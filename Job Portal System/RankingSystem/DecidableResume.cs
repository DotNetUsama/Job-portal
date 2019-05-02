using Job_Portal_System.Models;
using Job_Portal_System.RankingSystem.Abstracts;

namespace Job_Portal_System.RankingSystem
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
