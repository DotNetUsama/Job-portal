using System;
using System.Collections.Generic;
using Accord.MachineLearning.DecisionTrees;
using Accord.Math;

namespace Job_Portal_System.RankingSystem.Abstracts
{
    [Serializable]
    public class OurDecisionTree : DecisionTree
    {
        public OurDecisionTree(DecisionVariable[] attributes, int classCount) : base(attributes, classCount)
        {

        }

        public int[] OurDecide(double[] input)
        {
            return OurDecide(input, Root);
        }

        public int[] OurDecide(int[] input)
        {
            return OurDecideIterative(input, Root);
        }

        public int[] OurDecide(int?[] input)
        {
            return OurDecide(input, Root);
        }

        public int[] OurDecide(int?[] input, DecisionNode subtree)
        {
            return OurDecide(input.Apply(x => x == null ? double.NaN : (double)x), subtree);
        }

        public int[] OurDecide(double[] input, DecisionNode subtree)
        {
            if (subtree == null)
                throw new ArgumentNullException(nameof(subtree));

            if (subtree.Owner != this)
                throw new ArgumentException("The node does not belong to this tree.", nameof(subtree));

            return OurDecideIterative(input, subtree);
        }

        private static int[] OurDecideIterative(double[] input, DecisionNode subtree)
        {
            var current = subtree;
            var result = new List<int>();

            while (current != null)
            {
                if (current.IsLeaf)
                {
                    return result.ToArray();
                }

                var attribute = current.Branches.AttributeIndex;
                var value = input[attribute];

                DecisionNode nextNode = null;

                foreach (var branch in current.Branches)
                {
                    if (!branch.Compute(value)) continue;
                    nextNode = branch;
                    if (branch.Value > value) result.Add(attribute);
                    break;
                }

                current = nextNode;
            }

            throw new InvalidOperationException("The tree is degenerated. This is often a sign that "
                + "the tree is expecting discrete inputs, but it was given only real values.");
        }

        private static int[] OurDecideIterative(int[] input, DecisionNode subtree)
        {
            var current = subtree;
            var result = new List<int>();

            while (current != null)
            {
                if (current.IsLeaf)
                {
                    return result.ToArray();
                }

                var attribute = current.Branches.AttributeIndex;
                double value = input[attribute];
                result.Add(attribute);

                DecisionNode nextNode = null;

                foreach (DecisionNode branch in current.Branches)
                {
                    if (branch.Compute(value))
                    {
                        nextNode = branch; break;
                    }
                }

                current = nextNode;
            }

            throw new InvalidOperationException("The tree is degenerated. This is often a sign that "
                + "the tree is expecting discrete inputs, but it was given only real values.");
        }

    }
}
