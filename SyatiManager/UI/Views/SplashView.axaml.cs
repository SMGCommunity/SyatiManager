using Avalonia;
using Avalonia.Controls;

namespace SyatiManager.UI.Views {
    public partial class SplashView : UserControl {
        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<SplashView, double>(nameof(Value));

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<SplashView, string>(nameof(Message));

        public static readonly DirectProperty<SplashView, bool> IsFinishedProperty =
            AvaloniaProperty.RegisterDirect<SplashView, bool>(nameof(IsFinished), o => o.IsFinished);

        public double Value {
            get => GetValue(ValueProperty);
            set {
                SetValue(ValueProperty, value);
                RaisePropertyChanged(IsFinishedProperty, false, IsFinished);
            }
        }

        public string Message {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public bool IsFinished => Value >= 100;

        public SplashView() {
            InitializeComponent();
        }
    }
}