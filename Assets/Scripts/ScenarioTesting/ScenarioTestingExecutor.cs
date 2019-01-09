using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ScenarioTesting;
using Assets.Scripts.Units;
using Assets.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.ScenarioTesting
{
    public class ScenarioTestingExecutor : MonoBehaviour
    {
        public TestingScreenView TestingScreenView;
        public ScenarioTestingUIController Controller;
        public Scenario SingleScenario;
        public bool RunOnlySingleScenario = false;
        public bool ShouldShowAnimations = true;

        private Queue<ScenarioWithIndex> _scenarios;
        private List<ScenarioTestResult> _results = new List<ScenarioTestResult>();
        private ExecutorWithScenario _currentTest;
        private bool _testingEnded = false;

        public void Start()
        {
            Assert.raiseExceptions = true;
            //Controller.ShouldShowAnimations = ShouldShowAnimations;
            if (RunOnlySingleScenario)
            {
                _scenarios = new Queue<ScenarioWithIndex>();
                _scenarios.Enqueue(new ScenarioWithIndex()
                {
                    Index = 0,
                    Scenario = SingleScenario
                });
            }
            else
            {
                _scenarios = new Queue<ScenarioWithIndex>(
                    Resources.LoadAll<Scenario>("Testing/Scenarios").Cast<Scenario>().Where(c => c.TestEnabled).Select((c, i) => new ScenarioWithIndex()
                    {
                        Index = i,
                        Scenario = c
                    }).ToList()
                );
            }
        }

        public void Update()
        {
            if (_testingEnded)
            {
                return;
            }

            if (_currentTest == null)
            {
                if (_scenarios.Any())
                {
                    var scenarioWithIndex = _scenarios.Dequeue();
                    _currentTest = new ExecutorWithScenario()
                    {
                        Executor = new ScenarioOneTestExecutor(Controller, scenarioWithIndex.Scenario),
                        ScenarioWithIndex = scenarioWithIndex
                    };
                    TestingScreenView.ShowTestTitle("Test "+scenarioWithIndex.Index+": "+scenarioWithIndex.Scenario.FileName_DO_NOT_CHANGE_BY_HAND+" - "+scenarioWithIndex.Scenario.TestDescription);
                }
                else
                {
                    _testingEnded = true;
                    WriteTestsDescription();
                }
            }
            else
            {
                try
                {
                    _currentTest.Executor.UpdateScenario();
                }
                catch (Exception e)
                {
                    _results.Add(new ScenarioTestResult()
                    {
                        FailureException = e,
                        ScenarioWithIndex = _currentTest.ScenarioWithIndex
                    });
                    _currentTest.Executor.CleanUpState();
                    _currentTest = null;
                    return;
                }
                if (_currentTest.Executor.ExecutionFinished)
                {
                    _results.Add(new ScenarioTestResult()
                    {
                        ScenarioWithIndex = _currentTest.ScenarioWithIndex
                    });
                    _currentTest.Executor.CleanUpState();
                    _currentTest = null;
                }
            }    
        }

        private void WriteTestsDescription()
        {
            var executedTestsCount = _results.Count;
            var sb = new StringBuilder();
            sb.Append($"Executed {executedTestsCount} tests. ");
            if (_results.All(c => c.FailureException == null))
            {
                sb.Append($"All were OK");
                Debug.Log(sb.ToString());
                TestingScreenView.ShowTestTitle("Testing finished: OK");
            }
            else
            {
                var failedTests = _results.Where(c => c.FailureException != null).ToList();
                sb.AppendLine($"Of them {failedTests.Count} failed");
                foreach (var failedTest in failedTests)
                {
                    sb.AppendLine("-----------------------------");
                    sb.AppendLine($"Test {failedTest.ScenarioWithIndex.Index}: {failedTest.ScenarioWithIndex.Scenario.FileName_DO_NOT_CHANGE_BY_HAND} - {failedTest.ScenarioWithIndex.Scenario.TestDescription} with message:");
                    sb.Append(failedTest.FailureException.ToString());
                }
                Debug.LogError(sb.ToString());
                TestingScreenView.ShowTestTitle("Testing finished: Failure");
            }

        }
    }

    public class ScenarioTestResult
    {
        public ScenarioWithIndex ScenarioWithIndex;
        public Exception FailureException;
    }

    public class ScenarioWithIndex
    {
        public Scenario Scenario;
        public int Index; 
    }

    public class ExecutorWithScenario
    {
        public ScenarioOneTestExecutor Executor;
        public ScenarioWithIndex ScenarioWithIndex;
    }
    
}
