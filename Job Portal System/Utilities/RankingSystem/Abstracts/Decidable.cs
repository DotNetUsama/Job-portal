namespace Job_Portal_System.Utilities.RankingSystem.Abstracts
{
    public abstract class Decidable
    {
        public abstract object[] GetRow();

        public object GetValue(int parameterIndex)
        {
            return GetRow()[parameterIndex];
        }
    }
}