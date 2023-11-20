using essentialMix.Patterns.NotifyChange;
using JetBrains.Annotations;

namespace Test.WPF.ViewModels;

public abstract class ViewModelBase : NotifyPropertyChangedBase
{
	protected ViewModelBase()
	{
		Title = GetType().Name.Replace("ViewModel", string.Empty);
	}

	[NotNull]
	public string Title { get; }

	public abstract void Generate();
	public abstract void Clear();
}