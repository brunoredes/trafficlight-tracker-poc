using TrafficLight.ViewModels;

namespace TrafficLight.Views;

public partial class TrafficLightColorView : ContentPage
{
	public TrafficLightColorView()
	{
		InitializeComponent();
		BindingContext = new TrafficLightViewModel();
    }
}