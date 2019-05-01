using Job_Portal_System.Models;
using Job_Portal_System.RankingSystem.Abstracts;

namespace Job_Portal_System.RankingSystem
{
    internal class DecidableResume : Decidable
    {
        private readonly double[] _row;
        private bool _accepted;

        public DecidableResume(double[] row)
        {
            _row = row;
            _accepted = false;
        }

        public void Accept()
        {
            _accepted = true;
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
