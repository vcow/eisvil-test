using GameScene.Logic;

namespace GameScene.Quest.Controller
{
	public sealed class BlueEnemiesKilledQuestController : EnemiesKilledQuestControllerBase
	{
		public BlueEnemiesKilledQuestController(int numEnemies, SceneContext sceneContext)
			: base(numEnemies, sceneContext, EnemyType.Blue, QuestTriggerType.BlueEnemiesKilled)
		{
		}
	}
}