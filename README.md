# How to animate grid cell when underline changes in .NET MAUI DataGrid?

In this article, we will show you how to animate grid cell when underline changes in [.NET MAUI DataGrid](https://www.syncfusion.com/maui-controls/maui-datagrid).

## Xaml :

Use SfDataGrid with [CellTemplate](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.DataGrid.DataGridColumn.html#Syncfusion_Maui_DataGrid_DataGridColumn_CellTemplate) to host a Grid per cell and attach a custom AnimateBehavior. The behavior listens to changes on a specific bound property and briefly highlights the cell (fade-in/fade-out) to draw attention to updates.

```xml
<ContentPage.Content>
    <syncfusion:SfDataGrid x:Name="dataGrid"
                           HeaderGridLinesVisibility="Both"
                           ItemsSource="{Binding Stocks}">
        <syncfusion:SfDataGrid.Columns>
            <syncfusion:DataGridTextColumn HeaderText="Symbol" MappingName="Symbol" Width="120" CellTextAlignment="Center" HeaderTextAlignment="Center"/>
            <syncfusion:DataGridTextColumn HeaderText="Account" MappingName="Account" Width="180" CellTextAlignment="Center" HeaderTextAlignment="Center"/>
            <syncfusion:DataGridNumericColumn  HeaderText="Last Trade" MappingName="LastTrade" Width="140">
                <syncfusion:DataGridNumericColumn.CellTemplate>
                    <DataTemplate>
                        <Grid BackgroundColor="Transparent">
                            <Grid.Behaviors>
                                <local:AnimateBehavior WatchedProperty="LastTrade" />
                            </Grid.Behaviors>
                            <Label Text="{Binding LastTrade, StringFormat='{}{0:F2}'}" VerticalOptions="Center" HorizontalOptions="Center"/>
                        </Grid>
                    </DataTemplate>
                </syncfusion:DataGridNumericColumn.CellTemplate>
            </syncfusion:DataGridNumericColumn>

            <syncfusion:DataGridNumericColumn  HeaderText="Change" MappingName="Change" Width="120">
                <syncfusion:DataGridNumericColumn.CellTemplate>
                    <DataTemplate>
                        <Grid BackgroundColor="Transparent">
                            <Grid.Behaviors>
                                <local:AnimateBehavior WatchedProperty="Change" />
                            </Grid.Behaviors>
                            <Label Text="{Binding Change, StringFormat='{}{0:+0.##;-0.##;0}'}" VerticalOptions="Center" HorizontalOptions="Center"/>
                        </Grid>
                    </DataTemplate>
                </syncfusion:DataGridNumericColumn.CellTemplate>
            </syncfusion:DataGridNumericColumn>

            <syncfusion:DataGridNumericColumn  HeaderText="Open" MappingName="Open" Width="120">
                <syncfusion:DataGridNumericColumn.CellTemplate>
                    <DataTemplate>
                        <Grid BackgroundColor="Transparent">
                            <Grid.Behaviors>
                                <local:AnimateBehavior WatchedProperty="Open" />
                            </Grid.Behaviors>
                            <Label Text="{Binding Open, StringFormat='{}{0:F2}'}" VerticalOptions="Center" HorizontalOptions="Center"/>
                        </Grid>
                    </DataTemplate>
                </syncfusion:DataGridNumericColumn.CellTemplate>
            </syncfusion:DataGridNumericColumn>

            <syncfusion:DataGridNumericColumn  HeaderText="Prev Close" MappingName="PreviousClose" Width="120">
                <syncfusion:DataGridNumericColumn.CellTemplate>
                    <DataTemplate>
                        <Grid BackgroundColor="Transparent">
                            <Grid.Behaviors>
                                <local:AnimateBehavior WatchedProperty="PreviousClose" />
                            </Grid.Behaviors>
                            <Label Text="{Binding PreviousClose, StringFormat='{}{0:F2}'}" VerticalOptions="Center" HorizontalOptions="Center"/>
                        </Grid>
                    </DataTemplate>
                </syncfusion:DataGridNumericColumn.CellTemplate>
            </syncfusion:DataGridNumericColumn>
        </syncfusion:SfDataGrid.Columns>
    </syncfusion:SfDataGrid>
</ContentPage.Content>
```

## C#:

AnimateBehavior is a Behavior<Grid> that subscribes to the row item’s INotifyPropertyChanged. When the WatchedProperty changes, it animates the cell’s background color from the original to a highlight and back.

```c#
public class AnimateBehavior : Behavior<Grid>
{
    private void Context_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (grid == null || string.IsNullOrEmpty(WatchedProperty)) return;
        if (e.PropertyName == WatchedProperty)
            _ = AnimateAsync(grid);
    }

    private async Task AnimateAsync(Grid target)
    {
        if (isAnimating) return;
        isAnimating = true;

        var original = target.BackgroundColor ?? Colors.Transparent;

        Color highlight = HighlightColor;
        if (target.BindingContext is Stock s)
        {
            if (string.Equals(WatchedProperty, nameof(Stock.Change), StringComparison.Ordinal))
            {
                highlight = s.Change > 0 ? PositiveColor : (s.Change < 0 ? NegativeColor : NeutralColor);
            }
            else
            {
                highlight = PositiveColor;
            }
        }

        try
        {
            await ColorToAsync(target, original, highlight, TimeSpan.FromMilliseconds(300), Easing.CubicIn);
            await ColorToAsync(target, highlight, original, TimeSpan.FromMilliseconds(FadeOutMilliseconds), Easing.CubicOut);
        }
        finally
        {
            target.BackgroundColor = original;
            isAnimating = false;
        }
    }

    private static Task ColorToAsync(VisualElement element, Color from, Color to, TimeSpan length, Easing? easing = null)
    {
        TaskCompletionSource<bool> tcs = new();
        var animation = new Animation(v =>
        {
            var r = from.Red + (to.Red - from.Red) * v;
            var g = from.Green + (to.Green - from.Green) * v;
            var b = from.Blue + (to.Blue - from.Blue) * v;
            var a = from.Alpha + (to.Alpha - from.Alpha) * v;
            element.BackgroundColor = new Color((float)r, (float)g, (float)b, (float)a);
        }, 0, 1);

        animation.Commit(element, Guid.NewGuid().ToString(), 16, (uint)length.TotalMilliseconds, easing ?? Easing.Linear,
            (v, c) => tcs.TrySetResult(true));

        return tcs.Task;
    }
}
```

You can download this example on [GitHub](https://github.com/SyncfusionExamples/How-to-animate-grid-cell-when-underline-changes-in-.NET-MAUI-DataGrid).