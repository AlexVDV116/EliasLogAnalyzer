using Microsoft.Maui.Controls;

namespace EliasLogAnalyzer.MAUI.Resources.Behaviors
{
    public class ChangeIconBehavior : Behavior<Image>
    {
        public ImageSource? IconOne { get; set; }
        public ImageSource? IconTwo { get; set; }

        private TapGestureRecognizer? _tapGestureRecognizer;
        private bool _isIconOne = true; // State tracker

        protected override void OnAttachedTo(Image bindable)
        {
            base.OnAttachedTo(bindable);
            _tapGestureRecognizer = new TapGestureRecognizer();
            _tapGestureRecognizer.Tapped += OnTapped;
            bindable.GestureRecognizers.Add(_tapGestureRecognizer);
        }

        protected override void OnDetachingFrom(Image bindable)
        {
            if (_tapGestureRecognizer != null)
            {
                _tapGestureRecognizer.Tapped -= OnTapped;
                bindable.GestureRecognizers.Remove(_tapGestureRecognizer);
            }
            base.OnDetachingFrom(bindable);
        }

        private void OnTapped(object? sender, EventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;
            
            // Switch between IconOne and IconTwo
            image.Source = _isIconOne ? IconTwo : IconOne;
            _isIconOne = !_isIconOne; // Toggle state
        }
    }
}
