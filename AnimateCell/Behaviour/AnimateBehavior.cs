using AnimateCell.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AnimateCell.Behaviour
{
    public class AnimateBehavior : Behavior<Grid>
    {
        public string WatchedProperty { get; set; } = string.Empty;
        public int FadeInMilliseconds { get; set; } = 600;
        public int FadeOutMilliseconds { get; set; } = 600;
        public Color HighlightColor { get; set; } = Colors.DarkSlateBlue;
        public Color PositiveColor { get; set; } = Color.FromArgb("#5BAEA4");
        public Color NegativeColor { get; set; } = Color.FromArgb("#E46C6C");
        public Color NeutralColor { get; set; } = Color.FromArgb("#E9EEF0");

        private Grid? grid;
        private INotifyPropertyChanged? context;
        private bool isAnimating;

        protected override void OnAttachedTo(Grid bindable)
        {
            base.OnAttachedTo(bindable);
            grid = bindable;
            bindable.BindingContextChanged += Grid_BindingContextChanged;
            Wire(bindable.BindingContext);
        }

        protected override void OnDetachingFrom(Grid bindable)
        {
            base.OnDetachingFrom(bindable);
            Unwire();
            bindable.BindingContextChanged -= Grid_BindingContextChanged;
            grid = null;
        }

        private void Grid_BindingContextChanged(object? sender, EventArgs e)
        {
            Unwire();
            if (sender is Grid g)
                Wire(g.BindingContext);
        }

        private void Wire(object? ctx)
        {
            if (ctx is INotifyPropertyChanged npc)
            {
                context = npc;
                context.PropertyChanged += Context_PropertyChanged;
            }
        }

        private void Unwire()
        {
            if (context != null)
            {
                context.PropertyChanged -= Context_PropertyChanged;
                context = null;
            }
        }

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
                // Fade in to highlight, then fade out to original
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
}
