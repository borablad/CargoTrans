using CargoTrans.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrans.ViewModels
{
    public partial class LoginViewModel :BaseViewModel
    {
        [ObservableProperty]
        private string userLogin, userPassword;
        public LoginViewModel() 
        {
            //Login();
            UserLogin = Preferences.Get("login","");
            UserPassword = Preferences.Get("password","");
        }

        [RelayCommand]
        public async void Login()
        {
            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://ktzh.shit-systems.dev/api/");
            try
            {
                var loginRequest = new
                {
                    login = UserLogin,
                    password = UserPassword,
                    phone =""
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(loginRequest);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                // Аутентификация
                var authResponse = await _httpClient.PostAsync("auth/login", data);

                if (authResponse.IsSuccessStatusCode)
                {

                        var responseData = await authResponse.Content.ReadAsStringAsync();
                        var _authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseData);

                        if (await SaveTokensAsync(_authResponse.AccessToken, _authResponse.RefreshToken))
                        {
                            Preferences.Set("login",UserLogin);
                            Preferences.Set("password",UserPassword);
                            await AppShell.Current.GoToAsync($"{nameof(MainPage)}");

                        }

                        // Обработка полученных данных, если необходимо
                        return;
 
                }
                else
                {
                    await AppShell.Current.DisplayAlert("", "Failed to authenticate with the API. Status code: " + authResponse.StatusCode, "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("", "An error occurred: " + ex.Message, "OK");
                return;
            }
        
        }

       
        public async Task<bool> SaveTokensAsync(string accessToken, string refreshToken)
        {
            try
            {
                await SecureStorage.SetAsync("AccessToken", accessToken);
                await SecureStorage.SetAsync("RefreshToken", refreshToken);
                return true;
            }
            catch (Exception ex)
            {

                // Обработка ошибок сохранения токенов
                await AppShell.Current.DisplayAlert("", $"An error occurred while saving tokens: {ex.Message}", "ok");
                return false;
            }
        }
    }
    public class AuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
