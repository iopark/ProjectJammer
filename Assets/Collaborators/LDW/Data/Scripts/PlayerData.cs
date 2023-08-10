namespace LDW
{
    public class PlayerData
    {
        private PlayerData instance;
        public PlayerData Instance { get { return instance; } }

        public int viewId;
        public int hp;
        public int ammo;

        public PlayerData(int viewId, int hp, int ammo)
        {
            this.viewId = viewId;
            this.hp = hp;
            this.ammo = ammo;
        }
    }
}

