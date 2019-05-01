using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;

namespace Job_Portal_System.RankingSystem.Abstracts
{
    [Serializable]
    internal abstract class AbstractDecider
    {
        [NonSerialized]
        private Dictionary<string, Type> _parameters;
        [NonSerialized]
        private KeyValuePair<string, Type> _target;
        [NonSerialized]
        private readonly List<Decidable> _rows;
        private OurDecisionTree _tree;
        private DecisionVariable[] _attributes;

        protected AbstractDecider(Dictionary<string, Type> parameters, KeyValuePair<string, Type> target)
        {
            _parameters = parameters;
            _target = target;
            _rows = new List<Decidable>();
        }
        
        protected AbstractDecider(Dictionary<string, Type> parameters, 
            KeyValuePair<string, Type> target,
            OurDecisionTree tree)
        {
            _parameters = parameters;
            _target = target;
            _tree = tree;
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
            _rows.Add(row);
        }

        protected void AddRows(List<Decidable> rows)
        {
            _rows.AddRange(rows);
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

            foreach (KeyValuePair<string, Type> parameter in _parameters)
            {
                data.Columns.Add(parameter.Key, parameter.Value);
            }

            data.Columns.Add(_target.Key, _target.Value);

            foreach (var row in _rows)
            {
                data.Rows.Add(row.GetRow());
            }

            var codebook = new Codification(data);
            _attributes = GetDecisionVariables(codebook);

            var classCount = GetClassCount(codebook);

            _tree = new OurDecisionTree(_attributes, classCount);
            var c45 = new C45Learning(_tree);

            var symbols = codebook.Apply(data);
            var inputs = symbols.ToJagged(GetParameters());
            var outputs = symbols.ToArray<int>(GetTarget());

            c45.Learn(inputs, outputs);

            return _tree;
        }

        public IEnumerable<string> Infer(Decidable input)
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
            return output.Select(parameter => _attributes[parameter].Name);
        }

        private DecisionVariable[] GetDecisionVariables(Codification codebook)
        {
            var result = new DecisionVariable[_parameters.Count]; ;
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