using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Locomotion;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameCourseController : MonoBehaviour
    {
        public GameObject SpearmanPrefab; 
        public GameObject Elf1Prefab; 
        public GameObject Elf2Prefab; 
        public GameObject Elf3Prefab; 
        public GameObject Orc1Prefab; 
        public GameObject Orc2Prefab; 
        public GameObject Orc3Prefab; 

        public GameObject ArrowPrefab;
        public GameObject AxePrefab;
        public GameObject ProjectilePrefab; //uogolnienie

        private GameCourseModel _courseModel;
        private Stack<LocomotionManager<UnitModelComponent>> _unitLocomotions;
        private Stack<LocomotionManager<ProjectileModelComponent>> _projectileLocomotions;

        private Dictionary<UnitModel, UnitModelComponent> _unitModelToGameObjectMap = new Dictionary<UnitModel, UnitModelComponent>();
        private Dictionary<ProjectileModel, ProjectileModelComponent> _projectileModelToGameObjectMap = new Dictionary<ProjectileModel, ProjectileModelComponent>();

        public GameObject UnitsParent;
        public GameObject ProjectilesParent;

        public bool DebugShouldEndGame = true;

        public void Start()
        {
            _unitLocomotions = new Stack<LocomotionManager<UnitModelComponent>>();
            _projectileLocomotions = new Stack<LocomotionManager<ProjectileModelComponent>>();
            _courseModel = GetComponent<GameCourseModel>();
        }

        public GameCourseState CourseState
        {
            get
            {
                if (_courseModel.Phrase == Phrase.Placing)
                {
                    return GameCourseState.Starting;
                }
                if (_courseModel.Phrase == Phrase.Play && _courseModel.IsFinished() && DebugShouldEndGame)
                {
                    return GameCourseState.Finished;
                }
                if (_unitLocomotions.Any() || _projectileLocomotions.Any())
                {
                    return GameCourseState.NonInteractive;
                }
                else
                {
                    return GameCourseState.Interactive;
                }
            }
        }

        public void MyUpdate()
        {
            if (DebugShouldEndGame && _courseModel.Phrase == Phrase.Play && _courseModel.IsFinished()) //todo maybe Phrase.GameEnded?
            {
                return;
            }

            ProcessLocomotionStack(_projectileLocomotions);
            if (!_projectileLocomotions.Any())
            {
                ProcessLocomotionStack(_unitLocomotions);
            }
        }

        private ProjectileModelComponent AddProjectile(ProjectileCreations c)
        {
            var projectileModel = _courseModel.AddProjectile(c.StartPosition, c.Orientation);
            var projectileObject = GameObject.Instantiate(ArrowPrefab, ProjectilesParent.transform);
            var projectileModelComponent = projectileObject.GetComponent<ProjectileModelComponent>();
            projectileModelComponent.Model = projectileModel;
            _projectileModelToGameObjectMap[projectileModel] = projectileModelComponent;
            projectileModelComponent.Model.OnUnitKilled += () => _projectileModelToGameObjectMap.Remove(projectileModel);
            return projectileModelComponent;
        }

        public UnitModelComponent AddUnit(MyHexPosition startPosition, MyPlayer player, Orientation startOrientation, GameObject prefab) // public for test reasons
        {
            var unitModel = _courseModel.AddUnit(startPosition, player, startOrientation);
            var unitObject = GameObject.Instantiate(prefab, UnitsParent.transform);
            var unitModelComponent = unitObject.GetComponent<UnitModelComponent>();
            unitModelComponent.Model = unitModel;
            _unitModelToGameObjectMap[unitModel] = unitModelComponent;
            unitModelComponent.Model.OnUnitKilled += () => { _unitModelToGameObjectMap.Remove(unitModel); }; // quite hacky
            return unitModelComponent;
        }

        private void ProcessLocomotionStack<T>(Stack<LocomotionManager<T>> locomotions) where T : PawnModelComponent
        {
            if (locomotions.Any())
            {
                var currentLocomotion = locomotions.Peek();
                if (currentLocomotion.LocomotionFinished)
                {
                    locomotions.Pop();
                }
                else
                {
                    if (currentLocomotion.DuringAnimation)
                    {
                        currentLocomotion.UpdateAnimation();
                        return;
                    }
                    else
                    {
                        var steps = currentLocomotion.AdvanceJourney(_courseModel);
                        var previousStep = steps.PreviousStep;
                        var locomotionTarget = currentLocomotion.LocomotionTarget;
                        if (previousStep != null)
                        {
                            if (previousStep.ShouldRemoveUnitAfterStep(_courseModel))
                            {
                                locomotionTarget.PawnModel.SetUnitKilled();
                                locomotions.Pop();
                                return;
                            }
                        }

                        if (steps.NextStep != null)
                        {
                            var battleResults = steps.NextStep.ApplyStepToModel(_courseModel, locomotionTarget);

                            _unitLocomotions = new Stack<LocomotionManager<UnitModelComponent>>(_unitLocomotions
                                .Where(c => !battleResults.UnitWasStruck(  c.LocomotionTarget.Model  )).Reverse());

                            battleResults.KilledUnits.ForEach(c =>
                            {
                                _unitLocomotions.Push(LocomotionUtils.CreateDeathJourney( _unitModelToGameObjectMap[c] ));
                            });

                            battleResults.Displacements.ForEach(c => { _unitLocomotions.Push(LocomotionUtils.CreatePushJourney(_unitModelToGameObjectMap[c.Unit], c.DisplacementEnd)); });

                            battleResults.Projectiles.ForEach(c =>
                            {
                                var projectile = AddProjectile(c);
                                _projectileLocomotions.Push(LocomotionUtils.CreateProjectileJourney(projectile, c.EndPosition));
                            });
                        }
                    }
                }
            }
        }

        public void PlaceUnits() // temporary
        {
            // todo prawdziwa faza wystawiania
            AddUnit(new MyHexPosition(0, 0), MyPlayer.Player1, Orientation.N, Elf1Prefab);
            AddUnit(new MyHexPosition(1, 2), MyPlayer.Player1, Orientation.N, Elf2Prefab);
            AddUnit(new MyHexPosition(2, 4), MyPlayer.Player1, Orientation.N, Elf3Prefab);
            _courseModel.NextTurn();
            // todo wstawianie drugiego
            AddUnit(new MyHexPosition(4, 1), MyPlayer.Player2, Orientation.S, Orc1Prefab);
            AddUnit(new MyHexPosition(5, 2), MyPlayer.Player2, Orientation.S, Orc2Prefab);
            AddUnit(new MyHexPosition(5, 3), MyPlayer.Player2, Orientation.S, Orc3Prefab);
            _courseModel.Phrase = Phrase.Play;
            _courseModel.NextTurn();
            _courseModel.NextPhrase();
        }

        public void MoveTo(MyHexPosition selectorPosition, UnitModel selectedUnit)
        {
            _unitLocomotions.Push(LocomotionUtils.CreateMovementJourney(_unitModelToGameObjectMap[selectedUnit], selectorPosition));
            _courseModel.NextTurn();
        }

        public bool TileIsClickable(MyHexPosition hexPosition)
        {
            return _courseModel.HasTileAt(hexPosition);
        }

        public UnitModel GetUnitAtPlace(MyHexPosition position)
        {
            return _courseModel.GetUnitAt(position);
        }

        public MyPlayer CurrentPlayer => _courseModel.Turn.Player;

        public List<MyHexPosition> GetPossibleMoveTargets(UnitModel unit)
        {
            return unit.PossibleMoveTargets.Where(c => _courseModel.CanMoveTo(unit, c)).ToList();
        }

        public void NextPhrase()  //temporary, for testing
        {
            _courseModel.NextPhrase();
        } 

        public void Reset()
        {
            _unitLocomotions = new Stack<LocomotionManager<UnitModelComponent>>();
            _projectileLocomotions = new Stack<LocomotionManager<ProjectileModelComponent>>();
            _unitModelToGameObjectMap = new Dictionary<UnitModel, UnitModelComponent>();
            _projectileModelToGameObjectMap = new Dictionary<ProjectileModel, ProjectileModelComponent>();
            _courseModel.Reset();
        }
    }

    public enum GameCourseState
    {
        Starting, Finished, Interactive, NonInteractive
    }
}
