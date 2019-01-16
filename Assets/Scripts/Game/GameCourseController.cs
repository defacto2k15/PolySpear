using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Animation;
using Assets.Scripts.Locomotion;
using Assets.Scripts.Magic;
using Assets.Scripts.Map;
using Assets.Scripts.Sound;
using Assets.Scripts.Units;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.Game
{
    public class GameCourseController : MonoBehaviour
    {
        public GameObject ArrowPrefab;
        public GameObject AxePrefab;

        private GameCourseModel _courseModel;
        private Stack<IAnimation> _soloAnimations = new Stack<IAnimation>();
        private Stack<LocomotionManager<UnitModelComponent>> _unitLocomotions;
        private Stack<LocomotionManager<ProjectileModelComponent>> _projectileLocomotions;
        private MagicUsage _magicUsage;

        private Dictionary<PawnModel, UnitModelComponent> _unitModelToGameObjectMap = new Dictionary<PawnModel, UnitModelComponent>();
        private Dictionary<PawnModel, ProjectileModelComponent> _projectileModelToGameObjectMap = new Dictionary<PawnModel, ProjectileModelComponent>();

        public GameObject UnitsParent;
        public GameObject ProjectilesParent;
        public CameraShake CameraShake;
        public MasterSound MasterSound;
        private InitialGameStateCreator _initialGameStateCreator;

        public bool DebugShouldEndGame = true;

        public void Start()
        {
            _unitLocomotions = new Stack<LocomotionManager<UnitModelComponent>>();
            _projectileLocomotions = new Stack<LocomotionManager<ProjectileModelComponent>>();
            _courseModel = GetComponent<GameCourseModel>();

            _initialGameStateCreator = GetComponent<InitialGameStateCreator>();
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
                if (_unitLocomotions.Any() || _projectileLocomotions.Any() || _soloAnimations.Any())
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

        private PawnModelComponent GetPawnModelComponent(PawnModel model)
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

                                var innerAnimations = engagement.EngagementElements.Select(element =>
                                {
                                    var active = GetPawnModelComponent(element.ActivePawn);
                                    var passive = GetPawnModelComponent(element.PassivePawn);
                                    return element.EngagementVisibleConsequence.EngagementAnimation(_courseModel, MasterSound, active, passive);
                                }).ToList();
                                innerAnimations.AddRange(engagementResult.StruckUnits.Select(c => new UnitStruckAnimation( _unitModelToGameObjectMap[c])));

                                var anim = new CompositeAnimation(innerAnimations);

                                _soloAnimations.Push(anim);
                                if (_soloAnimations.Count >= 1)
                                {
                                    _soloAnimations.Peek().StartAnimation();
                                }


                                engagementResult.StruckUnits.ForEach(c =>
                                {
                                    _unitLocomotions.Push(LocomotionUtils.CreateDeathJourney(_unitModelToGameObjectMap[c]));
                                });

                                engagementResult.Displacements.ForEach(c =>
                                {
                                    _unitLocomotions.Push(LocomotionUtils.CreatePushJourney(_courseModel, _unitModelToGameObjectMap[c.Unit], c.DisplacementEnd));
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
            //// todo prawdziwa faza wystawiania
            _initialGameStateCreator.InitializePlayer1Units(this);
            _courseModel.NextTurn();
            // todo wstawianie drugiego
            _initialGameStateCreator.InitializePlayer2Units(this);
            _courseModel.Phrase = Phrase.Play;
            _courseModel.NextTurn();
            _courseModel.NextPhrase();
        }

        public void MoveTo(MyHexPosition selectorPosition, UnitModel selectedUnit, MyHexPosition magicUsePosition)
        {
            var magicType = _courseModel.GetPlayerMagicType(CurrentPlayer);
            if (magicUsePosition != null)
            {
                Assert.IsNull(_magicUsage);
                _magicUsage = new MagicUsage(magicType, magicUsePosition, _courseModel, CurrentPlayer, CameraShake, MasterSound);
            }
            _unitLocomotions.Push(LocomotionUtils.CreateMovementJourney(_courseModel, _unitModelToGameObjectMap[selectedUnit], selectorPosition));
            _courseModel.NextTurn();
            _possibleMoveTargetsCache = new Dictionary<PossibleMoveTargetsQueryKey, List<MyHexPosition>>();
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

        private class PossibleMoveTargetsQueryKey //todo TL4
        {
            private UnitModel _model;
            private MyHexPosition _position;

            public PossibleMoveTargetsQueryKey(UnitModel model, MyHexPosition position)
            {
                _model = model;
                _position = position;
            }

            protected bool Equals(PossibleMoveTargetsQueryKey other)
            {
                return Equals(_model, other._model) && Equals(_position, other._position);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PossibleMoveTargetsQueryKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_model != null ? _model.GetHashCode() : 0) * 397) ^ (_position != null ? _position.GetHashCode() : 0);
                }
            }
        }
        
        private Dictionary<PossibleMoveTargetsQueryKey, List<MyHexPosition>> _possibleMoveTargetsCache = new Dictionary<PossibleMoveTargetsQueryKey, List<MyHexPosition>>();

        public List<MyHexPosition> GetPossibleMoveTargets(UnitModel unit, MyHexPosition magicSelector)
        {
            var key = new PossibleMoveTargetsQueryKey(unit, magicSelector) ;
            if (_possibleMoveTargetsCache.ContainsKey(key))
            {
                return _possibleMoveTargetsCache[key];
            }
            var toReturn = unit.PossibleMoveTargets.Where(c => _courseModel.CanMoveTo(unit, c, magicSelector, CurrentPlayer)).ToList();
            _possibleMoveTargetsCache[key] = toReturn;
            return toReturn;
        }

        public List<MyHexPosition> GetPossibleMagicTargets(UnitModel unit)
        {
            return unit.PossibleMoveTargets.Where(c => _courseModel.CanMoveTo(unit, c, null, CurrentPlayer)).Where(c => _courseModel.CanUseMagicAt(c)).ToList();
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
            _possibleMoveTargetsCache = new Dictionary<PossibleMoveTargetsQueryKey, List<MyHexPosition>>();
            _courseModel.Reset();
        }

        public bool PlayerCanUseMagic()
        {
            return _courseModel.PlayerCanUseMagic(CurrentPlayer);
        }

        public void TestApplyWindMagic(MyHexPosition position)
        {
            _courseModel.UseMagic(MagicType.Wind, position, CurrentPlayer);
        }
    }

    public enum GameCourseState
    {
        Starting, Finished, Interactive, NonInteractive
    }
}
