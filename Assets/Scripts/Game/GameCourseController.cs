using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Locomotion;
using Assets.Scripts.Magic;
using Assets.Scripts.Map;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

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
        private Stack<IAnimation> _soloAnimations = new Stack<IAnimation>();
        private Stack<LocomotionManager<UnitModelComponent>> _unitLocomotions;
        private Stack<LocomotionManager<ProjectileModelComponent>> _projectileLocomotions;
        private MagicUsage _magicUsage;

        private Dictionary<PawnModel, UnitModelComponent> _unitModelToGameObjectMap = new Dictionary<PawnModel, UnitModelComponent>();
        private Dictionary<PawnModel, ProjectileModelComponent> _projectileModelToGameObjectMap = new Dictionary<PawnModel, ProjectileModelComponent>();

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

            var prefab = ArrowPrefab;
            if (c.Type == ProjectileType.Axe)
            {
                prefab = AxePrefab;
            }

            var projectileObject = GameObject.Instantiate(prefab, ProjectilesParent.transform);
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

        private PawnModelComponent getPawnModelComponent(PawnModel model)
        {
            if (_unitModelToGameObjectMap.ContainsKey(model))
            {
                return _unitModelToGameObjectMap[model];
            }else if (_projectileModelToGameObjectMap.ContainsKey(model))
            {
                return _projectileModelToGameObjectMap[model];
            }
            Assert.IsFalse(true, "There is not pawnModel in dictionaries: "+model);
            return null;
        }

        private void ProcessLocomotionStack<T>(Stack<LocomotionManager<T>> locomotions) where T : PawnModelComponent
        {
            if (_soloAnimations.Any())
            {
                var animation = _soloAnimations.Peek();
                animation.UpdateAnimation();
                if (!animation.WeAreDuringAnimation())
                {
                    _soloAnimations.Pop();
                    if (_soloAnimations.Any())
                    {
                        _soloAnimations.Peek().StartAnimation();
                    }
                }
                return;
            }

            if (_magicUsage != null)
            {
                if (_magicUsage.MagicUsageEnded)
                {
                    _magicUsage = null;
                }
                else
                {
                    _magicUsage.Update();
                    return;
                }
            };

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

                            foreach (var engagement in battleResults.Engagements)
                            {
                                var engagementResult =  engagement.EngagementResult;

                                var anim = new CompositeAnimation(engagement.EngagementElements.Select(element =>
                                {
                                    var active = getPawnModelComponent(element.ActivePawn);
                                    var passive = getPawnModelComponent(element.PassivePawn);
                                    return element.UsedEffect.UsageAnimationGenerator(_courseModel, active, passive);
                                }).ToList());
                                _soloAnimations.Push(anim);
                                if (_soloAnimations.Count == 1)
                                {
                                    _soloAnimations.Peek().StartAnimation();
                                }


                                engagementResult.StruckUnits.ForEach(c =>
                                {
                                    _unitLocomotions.Push(LocomotionUtils.CreateDeathJourney(_unitModelToGameObjectMap[c]));
                                });

                                engagementResult.Displacements.ForEach(c =>
                                {
                                    _unitLocomotions.Push(LocomotionUtils.CreatePushJourney(_unitModelToGameObjectMap[c.Unit], c.DisplacementEnd));
                                });

                                engagementResult.Projectiles.ForEach(c =>
                                {
                                    var projectile = AddProjectile(c);
                                    _projectileLocomotions.Push(LocomotionUtils.CreateProjectileJourney(projectile, c.EndPosition, c.Type));
                                });
                            }
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

        public void MoveTo(MyHexPosition selectorPosition, UnitModel selectedUnit, MyHexPosition magicUsePosition)
        {
            if (magicUsePosition != null)
            {
                Assert.IsNull(_magicUsage);
                _magicUsage = new MagicUsage(MagicType.Earth, magicUsePosition, _courseModel, CurrentPlayer);
            }
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
            _soloAnimations = new Stack<IAnimation>();
            _unitLocomotions = new Stack<LocomotionManager<UnitModelComponent>>();
            _projectileLocomotions = new Stack<LocomotionManager<ProjectileModelComponent>>();
            _unitModelToGameObjectMap = new Dictionary<PawnModel, UnitModelComponent>();
            _projectileModelToGameObjectMap = new Dictionary<PawnModel, ProjectileModelComponent>();
            _courseModel.Reset();
        }

        public bool PlayerCanUseMagic()
        {
            return _courseModel.PlayerCanUseMagic(CurrentPlayer);
        }
    }

    public enum GameCourseState
    {
        Starting, Finished, Interactive, NonInteractive
    }
}
