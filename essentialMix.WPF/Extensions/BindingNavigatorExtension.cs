using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace essentialMix.Extensions;

public static class BindingNavigatorExtension
{
	[NotNull]
	public static BindingNavigator AddCommonItems([NotNull] this BindingNavigator thisValue) { return AddCommonItems(thisValue, false, false); }
	[NotNull]
	public static BindingNavigator AddCommonItems([NotNull] this BindingNavigator thisValue, bool canEdit) { return AddCommonItems(thisValue, canEdit, canEdit); }
	[NotNull]
	public static BindingNavigator AddCommonItems([NotNull] this BindingNavigator thisValue, bool canAdd, bool CanDelete)
	{
		thisValue.MoveFirstItem = new ToolStripButton
		{
			Name = nameof(thisValue.MoveFirstItem),
			AccessibleName = "First",
			Text = "First",
			Image = IconChar.FastBackward.ToBitmap(IconFont.Auto, 18, SystemColors.ControlText),
			RightToLeftAutoMirrorImage = true,
			DisplayStyle = ToolStripItemDisplayStyle.Image
		};
		thisValue.Items.Add(thisValue.MoveFirstItem);

		thisValue.MovePreviousItem = new ToolStripButton
		{
			Name = nameof(thisValue.MovePreviousItem),
			AccessibleName = "Previous",
			Text = "Previous",
			Image = IconChar.StepBackward.ToBitmap(IconFont.Auto, 18, SystemColors.ControlText),
			RightToLeftAutoMirrorImage = true,
			DisplayStyle = ToolStripItemDisplayStyle.Image
		};
		thisValue.Items.Add(thisValue.MovePreviousItem);

		thisValue.Items.Add(new ToolStripSeparator());

		thisValue.PositionItem = new ToolStripTextBox
		{
			Name = nameof(thisValue.PositionItem),
			AccessibleName = "Position",
			ToolTipText = "Position",
			AutoToolTip = false,
			AutoSize = false,
			Width = 50,
			AutoCompleteMode = AutoCompleteMode.None,
			TextBoxTextAlign = HorizontalAlignment.Center
		};
		thisValue.Items.Add(thisValue.PositionItem);

		thisValue.CountItem = new ToolStripLabel
		{
			Name = nameof(thisValue.CountItem),
			ToolTipText = "Count",
			AutoToolTip = false
		};
		thisValue.Items.Add(thisValue.CountItem);

		thisValue.Items.Add(new ToolStripSeparator());

		thisValue.MoveNextItem = new ToolStripButton
		{
			Name = nameof(thisValue.MoveNextItem),
			AccessibleName = "Next",
			Text = "Next",
			Image = IconChar.StepForward.ToBitmap(IconFont.Auto, 18, SystemColors.ControlText),
			RightToLeftAutoMirrorImage = true,
			DisplayStyle = ToolStripItemDisplayStyle.Image
		};
		thisValue.Items.Add(thisValue.MoveNextItem);

		thisValue.MoveLastItem = new ToolStripButton
		{
			Name = nameof(thisValue.MoveLastItem),
			AccessibleName = "Last",
			Text = "Last",
			Image = IconChar.FastForward.ToBitmap(IconFont.Auto, 18, SystemColors.ControlText),
			RightToLeftAutoMirrorImage = true,
			DisplayStyle = ToolStripItemDisplayStyle.Image
		};
		thisValue.Items.Add(thisValue.MoveLastItem);

		if (canAdd || CanDelete) thisValue.Items.Add(new ToolStripSeparator());

		if (canAdd)
		{
			thisValue.AddNewItem = new ToolStripButton
			{
				Name = nameof(thisValue.AddNewItem),
				AccessibleName = "Add",
				Text = "Add",
				Image = IconChar.Plus.ToBitmap(IconFont.Auto, 18, SystemColors.ControlText),
				RightToLeftAutoMirrorImage = true,
				DisplayStyle = ToolStripItemDisplayStyle.Image
			};
			thisValue.Items.Add(thisValue.AddNewItem);
		}

		if (CanDelete)
		{
			thisValue.DeleteItem = new ToolStripButton
			{
				Name = nameof(thisValue.DeleteItem),
				AccessibleName = "Delete",
				Text = "Delete",
				Image = IconChar.Times.ToBitmap(IconFont.Auto, 18, SystemColors.ControlText),
				RightToLeftAutoMirrorImage = true,
				DisplayStyle = ToolStripItemDisplayStyle.Image
			};
			thisValue.Items.Add(thisValue.DeleteItem);
		}

		return thisValue;
	}
}