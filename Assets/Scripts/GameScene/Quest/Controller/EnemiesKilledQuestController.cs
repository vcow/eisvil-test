using GameScene.Logic;

namespace GameScene.Quest.Controller
{
	public sealed class EnemiesKilledQuestController : EnemiesKilledQuestControllerBase
	{
		public EnemiesKilledQuestController(int numEnemies, SceneContext sceneContext)
			: base(numEnemies, sceneContext, EnemyType.Undefined, QuestTriggerType.TotalEnemiesKilled)
		{
		}
	}
}