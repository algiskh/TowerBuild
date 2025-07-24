using UnityEngine;

public class MainSceneInstaller : BaseInstaller
{
	[SerializeField] private Camera _camera;
	[SerializeField] private RectTransform _dragContainer;
	[SerializeField] private ScrollPanel _scrollPanel;
	[SerializeField] private TowerView _towerView;
	[SerializeField] private GameLog _gameLog;

	public override void InstallBindings()
	{
		Container.BindInstance(_camera).WithId("mainCam");
		Container.BindInstance(_dragContainer).WithId("dragContainer");

		Bind(_towerView);
		Bind(_scrollPanel);
		Bind(_gameLog);
		Bind<InputManager>();
		Bind<TowerManager>();
		Bind<Localization.LocalizationManager>();
		//Bind<SerializationManager>();
	}
}