using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;

namespace Job_Portal_System.Utilities.RankingSystem.Abstracts
{
    internal abstract class AbstractDecider
    {
        private Dictionary<string, Type> _parameters;
        private KeyValuePair<string, Type> _target;
        private OurDecisionTree _tree;

        protected readonly List<object[]> Rows;
        protected DecisionVariable[] Attributes;

        protected AbstractDecider(Dictionary<string, Type> parameters, KeyValuePair<string, Type> target)
        {
            _parameters = parameters;
            _target = target;
            Rows = new List<object[]>();
        }

        protected abstract void AddNewRow(Decidable row);

        protected void AddNewRows(List<Decidable> rows)
        {
            foreach (var row in rows)
            {
                AddNewRow(row);
            }
        }

        protected void AddRow(Decidable row)
        {
            Rows.Add(row.GetRow());
        }

        protected void AddRows(List<Decidable> rows)
        {
            Rows.AddRange(rows.Select(r => r.GetRow()));
        }

        public void SetParametersAndTarget(Dictionary<string, Type> parameters,
            KeyValuePair<string, Type> target)
        {
            _parameters = parameters;
            _target = target;
        }

        public OurDecisionTree BuildDecisionTree()
        {
            var data = new DataTable();

            foreach (var parameter in _parameters)
            {
                data.Columns.Add(parameter.Key, parameter.Value);
            }

            data.Columns.Add(_target.Key, _target.Value);

            foreach (var row in Rows)
            {
                data.Rows.Add(row);
            }

            var codebook = new Codification(data);
            Attributes = GetDecisionVariables(codebook);

            var classCount = GetClassCount(codebook);

            _tree = new OurDecisionTree(Attributes, classCount);
            var c45 = new C45Learning(_tree);

            var symbols = codebook.Apply(data);
            var inputs = symbols.ToJagged(GetParameters());
            var outputs = symbols.ToArray<int>(GetTarget());

            c45.Learn(inputs, outputs);

            return _tree;
        }

        public List<string> Infer(Decidable input)
        {
            var data = new DataTable();

            foreach (var parameter in _parameters)
            {
                data.Columns.Add(parameter.Key, parameter.Value);
            }

            data.Columns.Add(_target.Key, _target.Value);
            data.Rows.Add(input.GetRow());

            var codebook = new Codification(data);

            var symbols = codebook.Apply(data);
            var inputRow = symbols.ToJagged(GetParameters())[0];

            var output = _tree.OurDecide(inputRow);

            return output.Select(parameter => Attributes[parameter].Name).ToList();
        }

        private DecisionVariable[] GetDecisionVariables(Codification codebook)
        {
            var result = new DecisionVariable[_parameters.Count]; 
            var i = 0;

            foreach (var parameter in _parameters)
            {
                var parameterType = parameter.Value;
                if (parameterType == typeof(int) ||
                    parameterType == typeof(float) ||
                    parameterType == typeof(double))
                {
                    result[i] = new DecisionVariable(parameter.Key, DecisionVariableKind.Continuous);
                }
                else
                {
                    var classCount = codebook[parameter.Key].NumberOfSymbols;
                    classCount = classCount == 1 ? classCount + 1 : classCount;
                    result[i] = new DecisionVariable(parameter.Key, classCount);
                }

                i++;
            }

            return result;
        }

        private int GetClassCount(Codification codebook)
        {
            return codebook[_target.Key].NumberOfSymbols;
        }

        private string[] GetParameters()
        {
            return _parameters.Keys.ToArray();
        }

        private string GetTarget()
        {
            return _target.Key;
        }
    }
}