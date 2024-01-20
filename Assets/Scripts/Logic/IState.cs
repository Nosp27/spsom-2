namespace Logic
{
    public interface IState
    {
        public void Tick();
        public void OnEnter();
        public void OnExit();

        public bool Done();
    }
}
