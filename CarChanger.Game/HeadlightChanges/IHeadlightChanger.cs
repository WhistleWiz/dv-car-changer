namespace CarChanger.Game.HeadlightChanges
{
    internal interface IHeadlightChanger
    {
        public HeadlightDirection Direction { get; set; }

        public void Apply();

        public void Unapply();
    }

    public enum HeadlightDirection
    {
        Front,
        Rear
    }
}
