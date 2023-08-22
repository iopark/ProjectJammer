namespace LDW
{
    public class PlayerData
    {
        private PlayerData instance;
        public PlayerData Instance { get { return instance; } }

        public int viewId;
        public int hp;
        public int ammo;
        public bool isAlive;

        public PlayerData(int viewId, int hp, int ammo, bool isAlive)
        {
            this.viewId = viewId;
            this.hp = hp;
            this.ammo = ammo;
            this.isAlive = isAlive;
        }
    }
}

