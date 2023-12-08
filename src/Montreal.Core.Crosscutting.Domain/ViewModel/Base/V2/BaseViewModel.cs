namespace Montreal.Core.Crosscutting.Domain.ViewModel.Base.V2
{
    public abstract class BaseViewModel<TSource, TDestination>
    {
        public abstract void LoadFrom(TSource source);

        public abstract TDestination Convert();

        public abstract void OverwriteDestination(TDestination destination);
    }
}
