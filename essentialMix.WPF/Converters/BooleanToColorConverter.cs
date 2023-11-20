using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using JetBrains.Annotations;

namespace essentialMix.WPF.Converters;

public class BooleanToBrushConverter : IValueConverter
{
	public BooleanToBrushConverter()
	{
	}

	public Brush TrueBrush { get; set; }
	public Brush FalseBrush { get; set; }

	/// <inheritdoc />
	[NotNull]
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return value is true
					? TrueBrush
					: FalseBrush;
	}

	/// <inheritdoc />
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}