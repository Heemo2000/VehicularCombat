using Game.StateMachineHandling;
using Game.Gameplay.Weapons;

namespace Game.Gameplay.EnemyManagement
{
    public class MachineGunEnemyFireState : IState
    {
        private MachineGunEnemy enemy;
        private AimHandler aimHandler;
        private Weapon weapon;
        public MachineGunEnemyFireState(MachineGunEnemy enemy)
        {
            this.enemy = enemy;
            this.aimHandler = this.enemy.AimHandler;
            this.weapon = this.enemy.Weapon;
        }

        public void OnEnter()
        {
            this.enemy.ApplyBrakes();
            this.aimHandler.AllowLookAtPosition = true;
        }

        public void OnUpdate()
        {
            this.aimHandler.AimPosition = this.enemy.Target.position;
            this.weapon.Fire();
        }

        public void OnFixedUpdate()
        {

        }

        public void OnLateUpdate()
        {

        }

        public void OnExit()
        {
            this.enemy.UnapplyBrakes();
            this.aimHandler.AllowLookAtPosition = false;
        }
    }
}
