namespace StepProgressBarSample
{
    public class StepProgressBarControl : StackLayout
    {
        private Button _lastStepSelected;
        public static readonly BindableProperty StepsProperty = BindableProperty.Create(nameof(Steps), typeof(int), typeof(StepProgressBarControl), 0);
        public static readonly BindableProperty StepSelectedProperty = BindableProperty.Create(nameof(StepSelected), typeof(int), typeof(StepProgressBarControl), 0, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepColorProperty = BindableProperty.Create(nameof(StepColor), typeof(Microsoft.Maui.Graphics.Color), typeof(StepProgressBarControl), Colors.Black, defaultBindingMode: BindingMode.TwoWay);

        public Color StepColor
        {
            get { return (Color)GetValue(StepColorProperty); }
            set { SetValue(StepColorProperty, value); }
        }

        public int Steps
        {
            get { return (int)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        public int StepSelected
        {
            get { return (int)GetValue(StepSelectedProperty); }
            set { SetValue(StepSelectedProperty, value); }
        }

        public StepProgressBarControl()
        {
            Orientation = StackOrientation.Vertical;
            //HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.Fill;
            Padding = new Thickness(10, 0);
            Spacing = 0;
            AddStyles();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == StepsProperty.PropertyName)
            {
                for (int i = 0; i < Steps; i++)
                {
                    var button = new Button()
                    {
                        Text = $"{i + 1}",
                        ClassId = $"{i + 1}",
                        Style = Resources["unSelectedStyle"] as Style
                    };

                    button.Clicked += Handle_Clicked;

                    this.Children.Add(button);

                    if (i < Steps - 1)
                    {
                        var separatorLine = new BoxView()
                        {
                            BackgroundColor = Colors.SkyBlue,
                            HeightRequest = 1,
                            WidthRequest = 2,
                            VerticalOptions = LayoutOptions.Fill,
                            HorizontalOptions = LayoutOptions.Center
                        };
                        this.Children.Add(separatorLine);
                    }
                }
            }
            else if (propertyName == StepSelectedProperty.PropertyName)
            {
                //var children = Children.First(p => (!string.IsNullOrEmpty(p.ClassId) && Convert.ToInt32(p.ClassId) == StepSelected));
                var children = Children.First(p => (!string.IsNullOrEmpty(p.AutomationId) && Convert.ToInt32(p.AutomationId) == StepSelected));
                if (children != null) SelectElement(children as Button);
            }
            else if (propertyName == StepColorProperty.PropertyName)
            {
                AddStyles();
            }
        }

        private void Handle_Clicked(object sender, EventArgs e)
        {
            SelectElement(sender as Button);
        }

        private void SelectElement(Button elementSelected)
        {
            if (_lastStepSelected != null) _lastStepSelected.Style = Resources["unSelectedStyle"] as Style;

            elementSelected.Style = Resources["selectedStyle"] as Style;

            StepSelected = Convert.ToInt32(elementSelected.Text);
            _lastStepSelected = elementSelected;
        }

        private void AddStyles()
        {
            var unselectedStyle = new Style(typeof(Button))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty,   Value = Colors.Transparent },
                    new Setter { Property = Button.BorderColorProperty,   Value = StepColor },
                    new Setter { Property = Button.TextColorProperty,   Value = StepColor },
                    new Setter { Property = Button.BorderWidthProperty,   Value = 0.5 },
                    new Setter { Property = Button.CornerRadiusProperty,   Value = 20 },
                    new Setter { Property = HeightRequestProperty,   Value = 40 },
                    new Setter { Property = WidthRequestProperty,   Value = 40 }
            }
            };

            var selectedStyle = new Style(typeof(Button))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty, Value = StepColor },
                    new Setter { Property = Button.TextColorProperty, Value = Colors.White },
                    new Setter { Property = Button.BorderColorProperty, Value = StepColor },
                    new Setter { Property = Button.BorderWidthProperty,   Value = 0.5 },
                    new Setter { Property = Button.CornerRadiusProperty,   Value = 20 },
                    new Setter { Property = HeightRequestProperty,   Value = 40 },
                    new Setter { Property = WidthRequestProperty,   Value = 40 },
                    new Setter { Property = Button.FontAttributesProperty,   Value = FontAttributes.Bold }
            }
            };

            Resources = new ResourceDictionary();
            Resources.Add("unSelectedStyle", unselectedStyle);
            Resources.Add("selectedStyle", selectedStyle);
        }
    }
}