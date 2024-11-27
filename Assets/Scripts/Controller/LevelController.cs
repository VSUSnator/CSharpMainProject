using System.Linq;
using Model;
using Model.Config;
using Model.Runtime;
using UnityEngine;
using Utilities;
using View;

namespace Controller
{
    public class LevelController : IPlayerUnitChoosingListener
    {
        private readonly RuntimeModel _runtimeModel;
        private readonly RootController _rootController;
        private readonly BotController _botController;
        private readonly SimulationController _simulationController;
        private readonly RootView _rootView;
        private readonly Gameplay3dView _gameplayView;
        private readonly Settings _settings;
        private readonly TimeUtil _timeUtil;

        // Добавим поля для координаторов
        private UnitCoordinator _playerUnitCoordinator;
        private UnitCoordinator _botUnitCoordinator;

        public LevelController(RuntimeModel runtimeModel, RootController rootController)
        {
            _runtimeModel = runtimeModel;
            _rootController = rootController;
            _botController = new BotController(OnBotUnitChosen);
            _simulationController = new(runtimeModel, OnLevelFinished);
            
            _rootView = ServiceLocator.Get<RootView>();
            _gameplayView = ServiceLocator.Get<Gameplay3dView>();
            _settings = ServiceLocator.Get<Settings>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();
        }

        public void StartLevel(int level)
        {
            ServiceLocator.RegisterAs(this, typeof(IPlayerUnitChoosingListener));
            
            _rootView.HideLevelFinished();

            Random.InitState(level);
            SetInitialMoney();
            var density = Random.Range(_settings.MapMinDensity, _settings.MapMaxDensity);
            var map = MapGenerator.Generate(_settings.MapWidth, _settings.MapHeight, density, level);
            _runtimeModel.Clear();
            _runtimeModel.Map = new Map(map, Settings.PlayersCount);
            _runtimeModel.Stage = RuntimeModel.GameStage.ChooseUnit;
            _runtimeModel.Bases[RuntimeModel.PlayerId] = new MainBase(_settings.MainBaseMaxHp);
            _runtimeModel.Bases[RuntimeModel.BotPlayerId] = new MainBase(_settings.MainBaseMaxHp);

            // Создаем экземпляры координаторов
            _playerUnitCoordinator = new UnitCoordinator(_runtimeModel, _timeUtil);
            _botUnitCoordinator = new UnitCoordinator(_runtimeModel, _timeUtil);

            // Передаем координаторы юнитам
            foreach (var unit in _runtimeModel.PlayersUnits[RuntimeModel.PlayerId])
            {
                unit.Initialize(_playerUnitCoordinator);
            }

            foreach (var unit in _runtimeModel.PlayersUnits[RuntimeModel.BotPlayerId])
            {
                unit.Initialize(_botUnitCoordinator);
            }

            _gameplayView.Reinitialize();
        }

        public void OnPlayersUnitChosen(UnitConfig unitConfig)
        {
            if (unitConfig.Cost > _runtimeModel.Money[RuntimeModel.PlayerId])
                return;
            
            SpawnUnit(RuntimeModel.PlayerId, unitConfig);
            TryStartSimulation();
        }

        private void OnBotUnitChosen(UnitConfig unitConfig)
        {
            SpawnUnit(RuntimeModel.BotPlayerId, unitConfig);
            TryStartSimulation();
        }

        private void SpawnUnit(int forPlayer, UnitConfig config)
        {
            var pos = _runtimeModel.Map.FindFreeCellNear(
                _runtimeModel.Map.Bases[forPlayer],
                _runtimeModel.RoUnits.Select(x => x.Pos).ToHashSet());
            
            var unit = new Unit(config, pos);
            _runtimeModel.Money[forPlayer] -= config.Cost;
            _runtimeModel.PlayersUnits[forPlayer].Add(unit);

            // Инициализируем юнит с соответствующим координатором
            if (forPlayer == RuntimeModel.PlayerId)
            {
                unit.Initialize(_playerUnitCoordinator);
            }
            else if (forPlayer == RuntimeModel.BotPlayerId)
            {
                unit.Initialize(_botUnitCoordinator);
            }
        }

        private void TryStartSimulation()
        {
            if (_runtimeModel.Money[RuntimeModel.PlayerId] < _settings.GetCheapestPlayerUnitCost() &&
                _runtimeModel.Money[RuntimeModel.BotPlayerId] < _settings.GetCheapestEnemyUnitCost())
            {
                _runtimeModel.Stage = RuntimeModel.GameStage.Simulation;
            }
        }

        private void SetInitialMoney()
        {
            var startMoney = _settings.BaseLevelMoney + _runtimeModel.Level * _settings.LevelMoneyIncrement;
            var botMoneyAdvantage = (_runtimeModel.Level + _settings.BotMoneyAdvantageLevelShift) *
                                    _settings.BotMoneyAdvantagePerLevel;
            _runtimeModel.SetMoneyForAll(startMoney, startMoney + botMoneyAdvantage);
        }

        private void OnLevelFinished(bool playerWon)
        {
            _runtimeModel.Stage = RuntimeModel.GameStage.Finished;
            _rootView.ShowLevelFinished(playerWon);
            _timeUtil.RunDelayed(5f, () => _rootController.OnLevelFinished(playerWon));
        }
    }
}