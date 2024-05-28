namespace EliasLogAnalyzer.MAUI.Resources.Behaviors
{
    public class RotateImageBehavior : Behavior<Image>
    {
        private TapGestureRecognizer? _tapGestureRecognizer;

        protected override void OnAttachedTo(Image bindable)
        {
            base.OnAttachedTo(bindable);
            _tapGestureRecognizer = new TapGestureRecognizer();
            _tapGestureRecognizer.Tapped += OnTapped;
            bindable.GestureRecognizers.Add(_tapGestureRecognizer);
        }

        protected override void OnDetachingFrom(Image bindable)
        {
            base.OnDetachingFrom(bindable);
            if (_tapGestureRecognizer != null)
            {
                _tapGestureRecognizer.Tapped -= OnTapped;
                bindable.GestureRecognizers.Remove(_tapGestureRecognizer);
            }
        }

        private async void OnTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            if (image != null)
            {
                if (image.RotationY == 0)
                {
                    await image.RotateYTo(180, 250); // Rotate to 180 degrees over 250 milliseconds
                }
                else
                {
                    await image.RotateYTo(0, 250); // Rotate back to 0 degrees
                }
            }
        }
    }
}
