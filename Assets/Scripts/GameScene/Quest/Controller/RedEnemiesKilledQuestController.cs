using GameScene.Logic;

namespace GameScene.Quest.Controller
{
	public sealed class RedEnemiesKilledQuestController : EnemiesKilledQuestControllerBase
	{
		public RedEnemiesKilledQuestController(int numEnemies, SceneContext sceneContext)
			: base(numEnemies, sceneContext, EnemyType.Red, QuestTriggerType.RedEnemiesKilled)
		{
		}
	}
}