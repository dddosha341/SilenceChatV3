//Реализация логики поведения на страничке
using Silence.Infrastructure.ViewModels;

namespace Silence.App.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel _viewModel;

    public WelcomePage()
	{
		InitializeComponent();

        BindingContext = _viewModel = MauiProgram.Services.GetRequiredService<WelcomeViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await _viewModel.InitializeAsync();
        }
        catch
        {
            // TODO: handle error here - suggest to update page
        }
    }
}